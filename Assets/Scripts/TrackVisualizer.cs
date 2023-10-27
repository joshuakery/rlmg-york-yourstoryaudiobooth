using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//https://github.com/omarvision/waveform-2D/blob/main/Track.cs

namespace JoshKery.York.AudioRecordingBooth
{
    /// <summary>
    /// Draws the audioSource.clip's sample data to a Texture2D
    /// 
    /// NOTE: There is a bug in UnityWebRequest which is resulting in AudioClip.length property returning DOUBLE the actual length.
    /// https://forum.unity.com/threads/audioclip-length-is-incorrect-when-loading-from-webrequest-getaudioclip.1082183/
    /// </summary>
    public class TrackVisualizer : MonoBehaviour
    {
        public int lineWidth = 2;
        public int gapWidth = 10;
        public float maxLineHeight = 0.75f;
        public float minLineHeight = 0.1f;

        public Color background = Color.black;
        public Color foreground = Color.yellow;

        [SerializeField]
        private AudioClipLoader audioClipLoader;

        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private UnityEngine.UI.RawImage rawImage;

        private int samplesize;
        private float[] samples = null;
        private float[] waveform = null;

        private void OnEnable()
        {
            if (audioClipLoader != null)
                audioClipLoader.ClipLoaded.AddListener(DrawWaveform);
        }

        private void OnDisable()
        {
            if (audioClipLoader != null)
                audioClipLoader.ClipLoaded.RemoveListener(DrawWaveform);
        }

        private void DrawWaveform()
        {
            if (audioSource != null)
            {
                Texture2D texwav = GetWaveform(rawImage.rectTransform.rect.size);

                rawImage.texture = texwav;
            }
        }

        private Texture2D GetWaveform(Vector2 texSize)
        {
            if (audioSource?.clip == null) { return null; }

            int height = (int)texSize.y;
            int width = (int)texSize.x;

            int halfheight = height / 2;
            float waveMaxHeight = (float)height * maxLineHeight;

            
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

            // Set waveform size to the max number of lines we can fit in the texture
            waveform = new float[(int)(width / (gapWidth + lineWidth))];

            // Get the sound data
            samplesize = audioSource.clip.samples * audioSource.clip.channels;
            samples = new float[samplesize];
            audioSource.clip.GetData(samples, 0);

            // Bug Workaround - act like the AudioClip is HALF the length it returns
            samplesize /= 2;
            samples = samples.Take(samples.Length / 2).ToArray();


            // Fill waveform array for each line we'll draw
            int packsize = (samplesize / width);
            for (int x = 0; x < waveform.Length; x++)
            {
                float total = 0;
                for (int w = 0; w < gapWidth + lineWidth; w++)
                {
                    total += Mathf.Abs(samples[((x * (gapWidth + lineWidth)) + w) * packsize]);
                }
                waveform[x] = total / (gapWidth + lineWidth);
            }

            // Map the sound data to texture
            // 1 - Clear
            Color[] pixels = Enumerable.Repeat(background, width * height).ToArray();
            tex.SetPixels(pixels);



            // 2 - Plot
            float waveformMax = waveform.Max();
            for (int x = 0; x < waveform.Length; x++)
            {
                float normalizedWaveHeight = waveform[x] / waveformMax;

                float lineHalfHeightAtX = normalizedWaveHeight * waveMaxHeight / 2f;

                if (lineHalfHeightAtX > minLineHeight)
                {
                    for (int linePixel = 0; linePixel < lineWidth; linePixel++)
                    {
                        for (int y = 0; y < lineHalfHeightAtX; y++)
                        {
                            tex.SetPixel((x * (gapWidth + lineWidth)) + linePixel, halfheight + y, foreground);
                            tex.SetPixel((x * (gapWidth + lineWidth)) + linePixel, halfheight - y, foreground);
                        }
                    }
                }
            }

            tex.Apply();

            return tex;
        }
    }
}

