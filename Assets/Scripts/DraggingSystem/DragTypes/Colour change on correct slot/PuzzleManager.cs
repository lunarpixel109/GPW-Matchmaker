using TMPro;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public TMP_Text instructionText;   // optional: leave empty to run without text
    public int totalSlots = 3;

    int filled;

    void Start() => SetText("Drag each item onto its matching slot.\nGreen means correct, red means wrong.");

    public void SlotFilled()
    {
        filled++;
        SetText(filled >= totalSlots ? "All placed. Nice work!" : $"Correct! {filled}/{totalSlots} placed.");
    }

    public void WrongSlot() => SetText("Not that slot. Try another.");

    void SetText(string msg)
    {
        if (instructionText) instructionText.text = msg;
    }
    
    public void SlotEmptied()
    {
        filled = Mathf.Max(0, filled - 1);
        SetText($"{filled}/{totalSlots} placed.");
    }
}