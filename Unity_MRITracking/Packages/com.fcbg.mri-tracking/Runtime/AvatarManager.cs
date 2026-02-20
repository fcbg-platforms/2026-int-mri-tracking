using UnityEngine;
using UnityAtoms.BaseAtoms;
using System;

public class AvatarManager : MonoBehaviour
{
	[Header("Male model")]
	[SerializeField] GameObject _maleModel;
	private Animator _maleAnimator;
	private SkinnedMeshRenderer _maleRenderer;
	private static readonly float _MALE_BASE_LENGHT = 180f; // Kevn mesured in Blender
	private static readonly float _MALE_HAND_LENGTH = 16.1f; //Kevn hand mesured
	[SerializeField] Transform _MHandBone;
	[SerializeField] GameObject _MrigTargetLArm;
	[SerializeField] GameObject _MrigTargetRArm;
	[SerializeField] GameObject _MrigTargetRFinger;
	[SerializeField] GameObject _MrigTargetLLeg;
	[SerializeField] GameObject _MrigTargetRLeg;
	[SerializeField] SkinnedMeshRenderer _maleMeshRenderer;

	[Space]
	[Header("Female model")]
	[SerializeField] GameObject _femaleModel;
	private Animator _femaleAnimator;
	private SkinnedMeshRenderer _femaleRenderer;
	private static readonly float _FEMALE_BASE_LENGHT = 165f; //Camila mesured in Blender
	private static readonly float _FEMALE_HAND_LENGTH = 13.7f; //Camila hand mesured
	[SerializeField] Transform _FHandBone;
	[SerializeField] GameObject _FrigTargetLArm;
	[SerializeField] GameObject _FrigTargetRArm;
	[SerializeField] GameObject _FrigTargetRFinger;
	[SerializeField] GameObject _FrigTargetLLeg;
	[SerializeField] GameObject _FrigTargetRLeg;
	[SerializeField] SkinnedMeshRenderer _femaleMeshRenderer;


	[Space]
	[Header("Settings SO")]
	[SerializeField] FloatReference _bodyWeightVariable;
	[SerializeField] FloatReference _avatarSizeVariable;
	[SerializeField] FloatReference _irlHandSizeVariable;
	[SerializeField] bool isInMriScene;

	[SerializeField] IntReference _avatarGenderReference;
	[SerializeField] ColorReference _colorReference;


	Animator _animator;
	float _baseLenght;
	float _iniAvatarHandLength;
	Transform _rHandBone;
	SkinnedMeshRenderer skinnedMeshRenderer;

	private void Awake()
	{
		_maleRenderer = _maleModel.GetComponentInChildren<SkinnedMeshRenderer>();
		_femaleRenderer = _femaleModel.GetComponentInChildren<SkinnedMeshRenderer>();

		//_maleAnimator = _maleModel.GetComponentInChildren<Animator>();
		//_femaleAnimator = _femaleModel.GetComponentInChildren<Animator>();
	}

	void OnEnable()
	{
		ChangeGender(_avatarGenderReference.Value);
		//SetSize(_avatarSizeVariable.Value);
		//ADD SetSizeFinger + ref to 2bonesikFinger_R_target
		//SetSizeIK(_avatarSizeVariable.Value);
		SetOriginToHip4MotionCapture(_maleModel.transform.parent, _avatarSizeVariable.Value);
		SetColor(_colorReference.Value);
		// MorphologyChange(_bodyWeightVariable.Value);
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="bodyWeight">must be between -1 (thin) and 1 (large). 0 is the standard avatar.
	/// to call on "on value changed" function of the slider
	/// </param>
	public void MorphologyChange(float bodyWeight)
	{
		if (bodyWeight > 0) //= larger side
		{
			skinnedMeshRenderer.SetBlendShapeWeight(0, 100 * Mathf.Abs(bodyWeight));
			skinnedMeshRenderer.SetBlendShapeWeight(1, 0);
		}
		else //=thiner side
		{
			skinnedMeshRenderer.SetBlendShapeWeight(1, 100 * Mathf.Abs(bodyWeight));
			skinnedMeshRenderer.SetBlendShapeWeight(0, 0);
		}

		//if (!isInMriScene)
		//_animator.SetFloat("Larger", bodyWeight);
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="size">.in cm. TO PUT ON CHANGE SCALE</param>
	public void SetSize(float size)
	{
		var localScale = size / _baseLenght;
		//To keep the head at the same place, the scale should be change on the parent and not on the character itself (origin of the character is on the foot)
		_maleModel.transform.parent.localScale = new Vector3(localScale, localScale, localScale);
		_femaleModel.transform.parent.localScale = new Vector3(localScale, localScale, localScale);
	}

	/// <summary>
	/// Adjust the Bones IK target to the new size of the character
	/// </summary>
	/// <param name="size">.in cm. TO PUT ON CHANGE SCALE</param>
	public void SetSizeIK(float size)
	{
		var localScaleFactor = size / _baseLenght;
		Vector3 _maleRef = _maleModel.transform.position;
		Vector3 _femaleRef = _femaleModel.transform.position;

		// 1. change legs target (gloab -X)

		Vector3 legReferencePosition = _MrigTargetLLeg.transform.position;
		float legDistance = Vector3.Distance(legReferencePosition, _maleRef);
		_MrigTargetLLeg.transform.position = new Vector3(
			legReferencePosition.x - (localScaleFactor * legDistance),
			legReferencePosition.y,
			legReferencePosition.z);
		legReferencePosition = _MrigTargetRLeg.transform.position;
		legDistance = Vector3.Distance(legReferencePosition, _maleRef);
		_MrigTargetRLeg.transform.position = new Vector3(
			legReferencePosition.x - (localScaleFactor * legDistance),
			legReferencePosition.y,
			legReferencePosition.z);

		// 1.bis same for woman model
		legReferencePosition = _FrigTargetLLeg.transform.position;
		legDistance = Vector3.Distance(legReferencePosition, _femaleRef);
		_FrigTargetLLeg.transform.position = new Vector3(
			legReferencePosition.x - (localScaleFactor * legDistance),
			legReferencePosition.y,
			legReferencePosition.z);
		legReferencePosition = _FrigTargetRLeg.transform.position;
		legDistance = Vector3.Distance(legReferencePosition, _femaleRef);
		_FrigTargetRLeg.transform.position = new Vector3(
			legReferencePosition.x - (localScaleFactor * legDistance),
			legReferencePosition.y,
			legReferencePosition.z);

		// 2. change arms target (locaL +Y)
		Vector3 armReferencePosition = _MrigTargetRArm.transform.position;
		float armDistance = Vector3.Distance(armReferencePosition, _maleRef);
		_MrigTargetRArm.transform.localPosition = new Vector3(
			_MrigTargetRArm.transform.localPosition.x,
			_MrigTargetRArm.transform.localPosition.y + (localScaleFactor * armDistance),
			_MrigTargetRArm.transform.localPosition.z);

		// 2.bis
		armReferencePosition = _FrigTargetRArm.transform.position;
		armDistance = Vector3.Distance(armReferencePosition, _femaleRef);
		_FrigTargetRArm.transform.localPosition = new Vector3(
			_FrigTargetRArm.transform.localPosition.x,
			_FrigTargetRArm.transform.localPosition.y + (localScaleFactor * armDistance),
			_FrigTargetRArm.transform.localPosition.z);

		// 3. change finger target + setting finger lenght (local +Y)
		Vector3 fingerReferencePosition = _MrigTargetRFinger.transform.position;
		float fingerDistance = Vector3.Distance(fingerReferencePosition, _maleRef);
		_MrigTargetRFinger.transform.localPosition = new Vector3(
			_MrigTargetRFinger.transform.localPosition.x,
			_MrigTargetRFinger.transform.localPosition.y + (localScaleFactor * fingerDistance),
			_MrigTargetRFinger.transform.localPosition.z);

		// 3.bis
		fingerReferencePosition = _FrigTargetRFinger.transform.position;
		fingerDistance = Vector3.Distance(fingerReferencePosition, _femaleRef);
		_FrigTargetRFinger.transform.localPosition = new Vector3(
			_FrigTargetRFinger.transform.localPosition.x,
			_FrigTargetRFinger.transform.localPosition.y + (localScaleFactor * fingerDistance),
			_FrigTargetRFinger.transform.localPosition.z);

		// 4. change left hand target (global +X)
		Vector3 handReferencePosition = _MrigTargetLArm.transform.position;
		float handDistance = Vector3.Distance(handReferencePosition, _maleRef);
		_MrigTargetLArm.transform.position = new Vector3(
			handReferencePosition.x + (localScaleFactor * handDistance),
			handReferencePosition.y,
			handReferencePosition.z);

		// 5. change finger target + setting finger lenght
		var initialHandScale = _rHandBone.localScale / localScaleFactor;
		var newHandScale = initialHandScale * _irlHandSizeVariable.Value / _iniAvatarHandLength;
		_rHandBone.localScale = newHandScale;
	}

	public void ChangeGender(int genderReference)
	{
		_maleModel.SetActive(genderReference == 0);
		_femaleModel.SetActive(genderReference == 1);

		switch (genderReference)
		{
			case 0:
				//_animator = _maleAnimator;
				_baseLenght = _MALE_BASE_LENGHT;
				_iniAvatarHandLength = _MALE_HAND_LENGTH;
				_rHandBone = _MHandBone;
				skinnedMeshRenderer = _maleRenderer;
				break;
			case 1:
				//_animator = _femaleAnimator;
				_baseLenght = _FEMALE_BASE_LENGHT;
				_iniAvatarHandLength = _FEMALE_HAND_LENGTH;
				_rHandBone = _FHandBone;
				skinnedMeshRenderer = _femaleRenderer;
				break;
			default:
				throw new NotImplementedException();
		}
	}

	public void SetColor(Color color)
	{
		Debug.Log("Color changed to " + color.ToString());
		_maleMeshRenderer.materials[0].SetColor("_DiffuseColor", color);
		_maleMeshRenderer.materials[1].SetColor("_DiffuseColor", color);
		_maleMeshRenderer.materials[2].SetColor("_DiffuseColor", color);
		_maleMeshRenderer.materials[3].SetColor("_DiffuseColor", color);
		_femaleMeshRenderer.materials[0].SetColor("_DiffuseColor", color);
		_femaleMeshRenderer.materials[1].SetColor("_DiffuseColor", color);
		_femaleMeshRenderer.materials[2].SetColor("_DiffuseColor", color);
		_femaleMeshRenderer.materials[3].SetColor("_DiffuseColor", color);
	}

	private void SetOriginToHip4MotionCapture(Transform go, float size) //ration floor to waist = 0.53 in anthropometry
	{
		go.position = new Vector3(go.position.x + size / 100 * 0.47f, go.position.y, go.position.z);
	}
}
