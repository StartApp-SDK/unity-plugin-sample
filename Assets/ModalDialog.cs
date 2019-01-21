using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;

public class ModalDialog : MonoBehaviour {
	public Button okButton;
    public Button cancelButton;
    public GameObject modalDialogObject;

    private static ModalDialog dialog;

    public static ModalDialog Instance() {
        if (!dialog) {
            dialog = FindObjectOfType(typeof (ModalDialog)) as ModalDialog;
            if (!dialog)
                Debug.LogError ("There needs to be one active GdrpDialog script on a GameObject in your scene.");
        }
        
        return dialog;
    }

	public void Choice(UnityAction okEvent, UnityAction cancelEvent) {
        modalDialogObject.SetActive(true);
        
        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(okEvent);
        okButton.onClick.AddListener(ClosePanel);
        
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(cancelEvent);
        cancelButton.onClick.AddListener(ClosePanel);

        okButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);
    }

    void ClosePanel() {
        modalDialogObject.SetActive(false);
    }
}
