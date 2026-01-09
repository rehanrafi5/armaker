using UnityEngine;

namespace ARMarker
{

    [CreateAssetMenu(
        menuName = ConstantStrings.MENU_ROOT
        + "Create WorkLayer SFX Choices")]
    public class WorkLayerSFXChoices : BaseWorkLayerChoices<SFXLayerData>
    {

        [SerializeField]
        private Sprite defaultSprite;

        public Sprite DefaultSprite => defaultSprite;

    }

}