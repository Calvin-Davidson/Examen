using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.Misc;
using TMPro;
using UnityEngine;

namespace Runtime.Renderers
{
    [RequireComponent(typeof(Numpad))]
    public class NumpadRenderer : MonoBehaviour
    {
        [SerializeField] private TextMeshPro successText;
        [SerializeField] private TextMeshPro failedText;
        [SerializeField] private TextMeshPro codeText;
        
        private Numpad _numpad;
        private int _numbersInserted = 0;

        private void Start()
        {
            _numpad = GetComponent<Numpad>();
            
            _numpad.onCodeFailed.called.AddListener(HandleCodeFailed);
            _numpad.onCodeSuccess.called.AddListener(HandleCodeSuccess);
            _numpad.onPadPressed.called.AddListener(HandlePadPressed);
        }

        private void HandlePadPressed()
        {
            _numbersInserted += 1;
            UpdateCodeText();
        }

        private void UpdateCodeText()
        {
            string text = "";
            for (int i = 0; i < 4; i++)
            {
                if (i < _numbersInserted) text += "* ";
                else text += "_ ";
            }

            codeText.text = text;
        }
        
        private void HandleCodeFailed()
        {
            StopAllCoroutines();
            _numbersInserted = 0;
            failedText.gameObject.SetActive(true);
            codeText.gameObject.SetActive(false);
            StartCoroutine(DisableAllResultTextsAfter());
        }

        private void HandleCodeSuccess()
        {
            StopAllCoroutines();
            codeText.gameObject.SetActive(false);
            successText.gameObject.SetActive(true);
            StartCoroutine(DisableAllResultTextsAfter());
        }

        private IEnumerator DisableAllResultTextsAfter(float delay = 1f)
        {
            yield return new WaitForSeconds(delay);
            failedText.gameObject.SetActive(false);
            successText.gameObject.SetActive(false);
            codeText.gameObject.SetActive(true);
            UpdateCodeText();
        }
    }
}
