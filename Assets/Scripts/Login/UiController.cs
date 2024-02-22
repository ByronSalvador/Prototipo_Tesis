using UnityEngine;

public class UiController : MonoBehaviour
{
    public TMPro.TMP_InputField passwordLoginInput;
    public TMPro.TMP_InputField passwordRegisterInput;
    public TMPro.TMP_InputField passwordValidateRegisterInput;

    public void ToggleInputType()
    {
        if (this.passwordLoginInput != null)
        {
            if (this.passwordLoginInput.contentType == TMPro.TMP_InputField.ContentType.Password)
            {
                this.passwordLoginInput.contentType = TMPro.TMP_InputField.ContentType.Standard;
            }
            else
            {
                this.passwordLoginInput.contentType = TMPro.TMP_InputField.ContentType.Password;
            }
        }

        if (this.passwordRegisterInput != null)
        {
            if (this.passwordRegisterInput.contentType == TMPro.TMP_InputField.ContentType.Password)
            {
                this.passwordRegisterInput.contentType = TMPro.TMP_InputField.ContentType.Standard;
            }
            else
            {
                this.passwordRegisterInput.contentType = TMPro.TMP_InputField.ContentType.Password;
            }
        }

        if (passwordValidateRegisterInput != null)
        {
            if (this.passwordValidateRegisterInput.contentType == TMPro.TMP_InputField.ContentType.Password)
            {
                this.passwordValidateRegisterInput.contentType = TMPro.TMP_InputField.ContentType.Standard;
            }
            else
            {
                this.passwordValidateRegisterInput.contentType = TMPro.TMP_InputField.ContentType.Password;
            }
        }

        this.passwordLoginInput.ForceLabelUpdate();
        this.passwordRegisterInput.ForceLabelUpdate();
        this.passwordValidateRegisterInput.ForceLabelUpdate();
    }
}