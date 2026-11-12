using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class LuckyNumberMachine : MonoBehaviour, IInteractable
{
    [field: SerializeField] public Transform interactingPoint { get ; set ; }
    [SerializeField] private TMP_InputField luckyNumberInputField;
    [SerializeField] private LuckyNumberManager luckyNumberManager;

    public UnityEvent OnLuckyNumberConfirmEvent;
    public UnityEvent OnInteractEvent;

    public void Interact(GameObject interactor)
    {
        OnInteractEvent?.Invoke();
    }

    public void ConfirmLuckyNumber()
    {
        if(luckyNumberInputField.text == "")
        {
            return;
        }

        if(int.TryParse(luckyNumberInputField.text, out int result)) luckyNumberManager.SetLuckyNumber(result);

        if(result < 0 || result > 719)
        {
            luckyNumberInputField.text = "";
            return;
        }
        
        OnLuckyNumberConfirmEvent?.Invoke();
    }
}

