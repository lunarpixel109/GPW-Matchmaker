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

    
    public class ScoreManager : MonoBehaviour {

        [SerializeField] private List<ScoreCondition> scoreConditions;

        private List<PropertyStore> playerProperties;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() {
            playerProperties = FindObjectsByType<PropertyStore>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
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

            return overallScore;
        }
    }
}
