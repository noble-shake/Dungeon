using UnityEngine;

public class AICharacterEquipmentManager : CharacterEquipmentManager
{
    AICharacterManager ai;

    public WeaponModelInstantiationSlot rightHandSlot;
    public WeaponModelInstantiationSlot leftHandWeaponSlot;
    public WeaponModelInstantiationSlot leftHandShieldSlot;

    [SerializeField] WeaponManager rightWeaponManager;
    [SerializeField] WeaponManager leftWeaponManager;

    public GameObject rightHandWeaponModel;

    public GameObject leftHandWeaponModel;

    public WeaponManager RightWeaponManager { get => rightWeaponManager; private set => rightWeaponManager = value; }
    public WeaponManager LeftWeaponManager { get => leftWeaponManager; private set => leftWeaponManager = value; }

    protected override void Awake()
    {
        ai = GetComponent<AICharacterManager>();

        LoadWeaponsOnBothHands();

    }

    protected override void Start()
    {

    }

    public void LoadWeaponsOnBothHands()
    {
        LoadRightWeapon();
        LoadLeftWeapon();
    }

    public void LoadRightWeapon()
    {
        if (ai.aiCharacterInventoryManager.currentRightHandWeapon != null)
        {
            if (rightHandSlot != null) rightHandSlot.UnLoadWeapon();

            rightHandWeaponModel = Instantiate(ai.aiCharacterInventoryManager.currentRightHandWeapon.weaponModel);
            rightHandSlot.LoadWeapon(rightHandWeaponModel);
            RightWeaponManager = rightHandWeaponModel.GetComponent<WeaponManager>();
            RightWeaponManager.SetWeaponDamage(ai, ai.aiCharacterInventoryManager.currentRightHandWeapon, 0, 0);

            if (RightWeaponManager.weaponTrail != null)
                GetComponent<WeaponTrailController>().RightParticle = RightWeaponManager.weaponTrail.GetComponent<ParticleController>();

        }
    }

    public void LoadLeftWeapon()
    {
        if (ai.aiCharacterInventoryManager.currentLeftHandWeapon != null)
        {
            if (leftHandWeaponSlot != null) leftHandWeaponSlot.UnLoadWeapon();

            if (leftHandShieldSlot != null) leftHandShieldSlot.UnLoadWeapon();


            leftHandWeaponModel = Instantiate(ai.aiCharacterInventoryManager.currentLeftHandWeapon.weaponModel);

            switch (ai.aiCharacterInventoryManager.currentLeftHandWeapon.weaponModelType)
            {
                case WeaponModelType.Weapon:
                    leftHandWeaponSlot.LoadWeapon(leftHandWeaponModel);
                    break;
                case WeaponModelType.Shield:
                    leftHandShieldSlot.LoadWeapon(leftHandWeaponModel);
                    break;
                //case WeaponModelType.WeaponSupport:
                default:
                    break;

            }
            LeftWeaponManager = leftHandWeaponModel.GetComponent<WeaponManager>();
            LeftWeaponManager.SetWeaponDamage(ai, ai.aiCharacterInventoryManager.currentLeftHandWeapon, 0, 0);

            if (leftWeaponManager.weaponTrail != null)
                GetComponent<WeaponTrailController>().LeftParticle = LeftWeaponManager.weaponTrail.GetComponent<ParticleController>();

        }
    }
}
