using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scoring {
    public enum ScoreLevel {
        Bad,
        OK,
        Good,
        Excellent,
        Perfect
    }

    [Serializable]
    public struct FinalScoreLevel {
        [Header("Display Information")]
        [Tooltip("The text displayed to the user when this level is acheived")] public string text;
        [Tooltip("the Colour of the text")] public Color colour;

        [Header("Technical Information")]
        [Header("The minimum score for this level. Must be between 0-1")] [Range(0, 1)] public float lowerBound;
        [Header("The amount of \"CCU\" that the player gains or looses when this level is acheived")] public int valueChange;

    }

    public class ScoreManager : MonoBehaviour {

        [SerializeField] private List<ScoreCondition> scoreConditions;

        private List<PropertyStore> playerProperties;

        public List<FinalScoreLevel> scoreLevels;
        


        public bool isScoring = false;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() {
            playerProperties = FindObjectsByType<PropertyStore>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
            scoreLevels = scoreLevels.OrderBy(x => x.lowerBound).ToList();

            
        }

        void Update() {
            if (isScoring) {
                GetPlayerCountChange();
                isScoring = false;
            }
        }

        float ScoreMatch(PropertyStore a, PropertyStore b) {

            List<ScoreLevel> scoreLevels = new List<ScoreLevel>();
            int maxScore = 0;

            foreach (ScoreCondition condition in scoreConditions) {
                maxScore += 4;
                PropertyValue aVal = a.GetPropertyValue(condition.property);
                PropertyValue bVal = b.GetPropertyValue(condition.property);
                switch (condition.property.propertyType) {
                    case PropertyType.String:
                    case PropertyType.Number:
                        float difference = Mathf.Abs(aVal.NumberValue - bVal.NumberValue);
                        if (difference > condition.okBounds) {
                            scoreLevels.Add(ScoreLevel.Bad);
                        } else if (difference <= condition.okBounds && difference > condition.goodBounds) {
                            scoreLevels.Add(ScoreLevel.OK);
                        } else if (difference <= condition.goodBounds && difference > condition.excellentBounds) {
                            scoreLevels.Add(ScoreLevel.Good);
                        } else if (difference > condition.excellentBounds) {
                            scoreLevels.Add(ScoreLevel.Excellent);
                        } else if (difference == 0) {
                            scoreLevels.Add(ScoreLevel.Perfect);
                        }
                        break;
                    case PropertyType.Bool:
                        if (aVal.BoolValue == bVal.BoolValue) {
                            scoreLevels.Add(ScoreLevel.Perfect);
                        } else {
                            scoreLevels.Add(ScoreLevel.Bad);
                        }
                        break;
                }
            }

            float totalScore = 0;
            foreach (ScoreLevel scoreLevel in scoreLevels) {
                totalScore += (int)scoreLevel;
            }

            return totalScore / maxScore;

        }



        public float GetOverallScore() {
            float overallScore = 0;
            for (int i = 0; i < playerProperties.Count() - 1; ++i) {
                if (i > 0) {
                    overallScore += ScoreMatch(playerProperties[i], playerProperties[i - 1]);
                }
                overallScore += ScoreMatch(playerProperties[i], playerProperties[i + 1]);
            }
            Debug.Log(overallScore);
            
            
            return overallScore;

        }

        public int GetPlayerCountChange() {
            float score = GetOverallScore();
            int playerCountChange = 0;

            for (int i = 0; i <= scoreLevels.Count - 2; ++i) {
                if (score >= scoreLevels[i].lowerBound && score < scoreLevels[i+1].lowerBound) {
                    playerCountChange = scoreLevels[i].valueChange;
                    break;
                } else if (i == scoreLevels.Count - 2) {
                    if (score >= scoreLevels[i + 1].lowerBound) {
                        playerCountChange = scoreLevels[i + 1].valueChange;
                    }
                }
            }
            
            Debug.Log(playerCountChange);
            return playerCountChange;
        }
    }
}
