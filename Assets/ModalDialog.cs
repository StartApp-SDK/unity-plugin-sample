using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;

public class ModalDialog : MonoBehaviour
{
    public Button mOkButton;
    public Button mCancelButton;
    public GameObject mModalDialogObject;

    private static ModalDialog dialog;

    public static ModalDialog Instance()
    {
        if (!dialog)
        {
            dialog = FindObjectOfType(typeof(ModalDialog)) as ModalDialog;
            if (!dialog)
                Debug.LogError("There needs to be one active GdrpDialog script on a GameObject in your scene.");
        }

        return dialog;
    }

    public void Choice(UnityAction okEvent, UnityAction cancelEvent)
    {
        mModalDialogObject.SetActive(true);

        mOkButton.onClick.RemoveAllListeners();
        mOkButton.onClick.AddListener(okEvent);
        mOkButton.onClick.AddListener(ClosePanel);

        mCancelButton.onClick.RemoveAllListeners();
        mCancelButton.onClick.AddListener(cancelEvent);
        mCancelButton.onClick.AddListener(ClosePanel);

        mOkButton.gameObject.SetActive(true);
        mCancelButton.gameObject.SetActive(true);
    }

    void ClosePanel()
    {
        mModalDialogObject.SetActive(false);
    }
}
