using System.Linq;
using UnityEngine;

//https://github.com/omarvision/waveform-2D/blob/main/Track.cs

namespace JoshKery.York.AudioRecordingBooth
{
    public class WaveformSaver : MonoBehaviour
    {
        /// <summary>
        /// Num stripes to appear across width of rawImage
        /// </summary>
        [SerializeField]
        private int nStripes = 50;

        [SerializeField]
        private AudioClipLoader audioClipLoader;

        [SerializeField]
        private AudioSource audioSource;



        /// <summary>
        /// Custom material with shader that will use this waveform data
        /// </summary>
        [SerializeField]
        private Material material;


        [SerializeField]
        private UnityEngine.UI.RawImage displayRawImage;
        

        private int samplesize;
        private float[] samples = null;
        private float[] waveform = null;

        [Header("Debug Options")]

        /// <summary>
        /// If true, saves a png of waveform tex to StreamingAssets/
        /// </summary>
        [SerializeField]
        private bool doSaveOut = false;

        /// <summary>
        /// Display for shader
        /// </summary>
        [SerializeField]
        private UnityEngine.UI.RawImage debugRawImage;

        private void OnEnable()
        {
            if (audioClipLoader != null)
                audioClipLoader.ClipLoaded.AddListener(SaveWaveform);
        }

        private void OnDisable()
        {
            if (audioClipLoader != null)
                audioClipLoader.ClipLoaded.RemoveListener(SaveWaveform);
        }

        private void SaveWaveform()
        {
            if (audioSource != null && displayRawImage != null)
            {
                int i_width = Mathf.RoundToInt(displayRawImage.rectTransform.rect.width);

                Texture2D texwav = GetWaveform(i_width, nStripes);

                

                if (material != null)
                {
                    material.SetFloat("_RectWidth", i_width);
                    material.SetFloat("_NumStripes", nStripes);
                    material.SetTexture("_WaveformTex", texwav);
                }

                if (displayRawImage != null)
                    displayRawImage.texture = null;

                //Debug
                if (doSaveOut)
                {
                    byte[] bytes = texwav.EncodeToPNG();
                    System.IO.File.WriteAllBytes(Application.streamingAssetsPath + "/test-waveform-output.png", bytes);
                }

                if (debugRawImage != null)
                    debugRawImage.texture = texwav;

            }
        }

        /// <summary>
        /// From an audio clip, gets the average sample size for portions of the clip
        /// and saves them to a Texture2D so they can be used as texture data in ShaderGraph
        /// </summary>
        /// <param name="rectWidth">Width of display RawImage</param>
        /// <param name="numStripes"></param>
        /// <returns></returns>
        private Texture2D GetWaveform(int rectWidth, int numStripes)
        {
            if (audioSource?.clip == null) { return null; }

            // Set waveform array size to the given number of lines
            waveform = new float[numStripes];

            //Prepare the Texture2D to hold this data
            Texture2D waveformTex = new Texture2D(numStripes, 1, TextureFormat.RGBA32, false);

            // Get the sound data
            samplesize = audioSource.clip.samples * audioSource.clip.channels;
            samples = new float[samplesize];
            audioSource.clip.GetData(samples, 0);

            // Bug Workaround - act like the AudioClip is HALF the length it returns
            samplesize /= 2;
            samples = samples.Take(samples.Length / 2).ToArray();

            //The size of ???
            int packsize = (samplesize / rectWidth);

            //The width of each stripe, both positive and negative visual space
            int repeatWidth = rectWidth / numStripes;

            // Fill waveform array with the average sample size for each line we'll draw
            for (int x = 0; x < waveform.Length; x++)
            {
                float total = 0;
                for (int sampleIndex = 0; sampleIndex < repeatWidth; sampleIndex++)
                {
                    total += Mathf.Abs(samples[ ( ( x * repeatWidth ) + sampleIndex ) * packsize ]);
                }

                //take the average sample size
                waveform[x] = total / repeatWidth;
            }

            // Map the sound data to texture
            float waveformMax = waveform.Max();
            for (int x = 0; x < waveform.Length; x++)
            {
                float normalizedWav = waveform[x] / waveformMax;

                waveformTex.SetPixel(x, 0, new Color(normalizedWav, normalizedWav, normalizedWav));
            }

            waveformTex.Apply();

            return waveformTex;
        }

        public void OnSaveWaveform()
        {
            SaveWaveform();
        }
    }

}
