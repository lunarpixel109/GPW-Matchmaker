using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scoring {
    public class OverallScoreManager : MonoBehaviour {
        
        List<LobbyScoreManager> scoreManagers = new List<LobbyScoreManager>();

        [SerializeField] public List<ScoreCondition> scoreConditions;

        [SerializeField] public GameObject scoreTextPrefab;
        [SerializeField] public TextMeshProUGUI overallScoreText;
        [SerializeField] public Vector2 transformOffsetToLobby;
        public List<FinalScoreLevel> finalScoreLevels;

        public InputAction forceScoring;

        public int currentCCU = 10000;

        private void Start() {

            scoreManagers = FindObjectsByType<LobbyScoreManager>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();

            foreach (LobbyScoreManager scoreManager in scoreManagers) {
                scoreManager.onScoringComplete += UpdateAndShowScore;
            }


            overallScoreText.text = currentCCU.ToString();

            forceScoring.Enable();
            forceScoring.performed += _ => Score();
        }


        private void UpdateAndShowScore(FinalScoreLevel ccuChange, Transform lobby) {
            // 1. Update the player score (stored in here for now)
            currentCCU += ccuChange.valueChange;

            // 2. Show new score on main score text
            overallScoreText.text = currentCCU.ToString();

            // 3. Show custom score change stamp
            StartCoroutine(ShowChangeInScore(ccuChange, lobby));


        }

        private IEnumerator ShowChangeInScore(FinalScoreLevel level, Transform lobby) {
            var newScoreText = Instantiate(scoreTextPrefab,  lobby);
            newScoreText.GetComponent<RectTransform>().anchoredPosition = lobby.GetComponent<RectTransform>().anchoredPosition + transformOffsetToLobby;
            newScoreText.GetComponentInChildren<TextMeshProUGUI>().text = level.ToString();
            newScoreText.GetComponentInChildren<TextMeshProUGUI>().color = level.colour;
            yield return new WaitForSeconds(1f);
            Destroy(newScoreText);
        }

        private void Score() {
        //    int playerChange = 0;
            foreach (var scoreManager in scoreManagers) {
                scoreManager.SubmitPressed();

            }
       //     Debug.Log(playerChange);
        }


    }
    
}