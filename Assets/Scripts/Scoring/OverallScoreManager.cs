using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scoring {
    public class OverallScoreManager : MonoBehaviour {
        
        List<LobbyScoreManager> scoreManagers = new List<LobbyScoreManager>();

        [SerializeField] private List<ScoreCondition> scoreConditions;

        public List<FinalScoreLevel> finalScoreLevels;

        public InputAction forceScoring;
        
        private void Start() {

            scoreManagers = FindObjectsByType<LobbyScoreManager>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
            
            forceScoring.Enable();
            forceScoring.performed += _ => Score();
        }


        private void Score() {
            int playerChange = 0;
            foreach (var scoreManager in scoreManagers) {
                playerChange += scoreManager.GetPlayerCountChange(scoreConditions, finalScoreLevels);
                
            }
            Debug.Log(playerChange);
        }

        private void ShowScores(int score) {
            
        }
    }
    
}