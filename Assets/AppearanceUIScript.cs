using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AppearanceUIScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI BodyTypeText;
    [SerializeField] TextMeshProUGUI FaceTypeText;
    [SerializeField] Slider RedSlider;
    [SerializeField] Slider GreenSlider;
    [SerializeField] Slider BlueSlider;

    private void OnEnable()
    {
        RedSlider.SetValueWithoutNotify(PlayerScript.PlayerCharacter.ColorSet.R);
        GreenSlider.SetValueWithoutNotify(PlayerScript.PlayerCharacter.ColorSet.G);
        BlueSlider.SetValueWithoutNotify(PlayerScript.PlayerCharacter.ColorSet.B);
        UpdateBodyTypeText();
        UpdateFaceTypeText();
    }

    private void UpdateBodyTypeText()
    {
        BodyTypeText.text = "Body Type " + (PlayerScript.PlayerCharacter.GetBodyID + 1) + "/" + PlayerScript.PlayerCharacter.GetBodyTypeCount;
    }

    private void UpdateFaceTypeText()
    {
        FaceTypeText.text = "Face Type " + (PlayerScript.PlayerCharacter.GetFaceID + 1) + "/" + PlayerScript.PlayerCharacter.GetFaceTypeCount;
    }

    public void OnClickPreviousBodyType()
    {
        PlayerScript player = PlayerScript.PlayerCharacter;
        if (player.GetBodyID == 0)
            player.ChangeBody((byte)(player.GetBodyTypeCount - 1));
        else
            player.ChangeBody((byte)(player.GetBodyID - 1));
        UpdateBodyTypeText();
    }

    public void OnClickNextBodyType()
    {
        PlayerScript player = PlayerScript.PlayerCharacter;
        if (player.GetBodyID == player.GetBodyTypeCount - 1)
            player.ChangeBody(0);
        else
            player.ChangeBody((byte)(player.GetBodyID + 1));
        UpdateBodyTypeText();
    }

    public void OnClickPreviousFaceType()
    {
        PlayerScript player = PlayerScript.PlayerCharacter;
        if (player.GetFaceID == 0)
            player.ChangeFace((byte)(player.GetFaceTypeCount - 1));
        else
            player.ChangeFace((byte)(player.GetFaceID - 1));
        UpdateFaceTypeText();
    }

    public void OnClickNextFaceType()
    {
        PlayerScript player = PlayerScript.PlayerCharacter;
        if (player.GetFaceID == player.GetFaceTypeCount - 1)
            player.ChangeFace(0);
        else
            player.ChangeFace((byte)(player.GetFaceID + 1));
        UpdateFaceTypeText();
    }

    public void OnUpdateSlider()
    {
        PlayerScript player = PlayerScript.PlayerCharacter;
        player.ColorSet.R = (byte)RedSlider.value;
        player.ColorSet.G = (byte)GreenSlider.value;
        player.ColorSet.B = (byte)BlueSlider.value;
        player.ChangeColor(player.ColorSet.ReturnColor(0.7f));
    }

    public void OnCloseInterfaceClicked()
    {
        PlayerScript.PlayerCharacter.SyncAppearance();
        transform.gameObject.SetActive(false);
    }
}
