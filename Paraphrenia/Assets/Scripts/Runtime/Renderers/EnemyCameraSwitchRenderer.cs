using System;
using Runtime.Player;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Runtime.Renderers
{
    [RequireComponent(typeof(EnemyCameraSwitcher))]
    public class EnemyCameraSwitchRenderer : MonoBehaviour
    {
        [SerializeField] private Volume processingVolume;
        [SerializeField] private float targetIntensity = 1;
        [SerializeField] private float targetSmoothness = 1;
        [SerializeField] private float targetRoundness = 1;

        private EnemyCameraSwitcher _enemyCameraSwitcher;
        private Vignette _vignette;
        private float _switchProgress = 0;

        private bool _isSwitching = false;

        private float _startIntensity;
        private float _startSmoothness;
        private float _startRoundness;

        private void Awake()
        {
            _enemyCameraSwitcher = GetComponent<EnemyCameraSwitcher>();

            _enemyCameraSwitcher.onSwitchComplete.AddListener(HandleSwitchComplete);
            _enemyCameraSwitcher.onSwitchCancelled.AddListener(HandleSwitchCancel);
            _enemyCameraSwitcher.onSwitchStart.AddListener(HandleSwitchStart);
            _enemyCameraSwitcher.onSwitchProgress.AddListener(HandleSwitchProgressChange);

            if (!processingVolume.profile) throw new NullReferenceException(nameof(VolumeProfile));

            if (!processingVolume.profile.TryGet(out Vignette vignette))
                throw new NullReferenceException(nameof(vignette));
            _vignette = vignette;

            _startIntensity = vignette.intensity.value;
            _startRoundness = vignette.roundness.value;
            _startSmoothness = vignette.smoothness.value;
        }


        private void Update()
        {
            if (!_isSwitching)
            {
                _switchProgress -= Time.deltaTime;
                _switchProgress = Mathf.Clamp01(_switchProgress);
            }

            // Easing
            float progress = _switchProgress > 1 ? 1f : 1f - Mathf.Pow(2f, -10f * _switchProgress);
            _vignette.intensity.Override(Mathf.Lerp(0, 1, progress));
            _vignette.roundness.Override(Mathf.Lerp(0, 1, progress));
            _vignette.smoothness.Override(Mathf.Lerp(0, 1, progress));
        }

        private void HandleSwitchComplete()
        {
            _isSwitching = false;
        }

        private void HandleSwitchCancel()
        {
            _isSwitching = false;
        }

        private void HandleSwitchStart()
        {
            _isSwitching = true;
        }

        private void HandleSwitchProgressChange(float progress)
        {
            _switchProgress = progress;
        }
        
    }
}