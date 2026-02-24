namespace MriManagement
{
	using EyapLibrary.Easings;
	using UnityAtoms.BaseAtoms;
	using UnityEngine;
	using UnityEngine.Animations;
	using System;
	using System.Collections;

	/// <summary>
	///  Specific action simulate the movement of the MRI table to enter it.
	///  You can use the motion tracking to have a real time movement or using a simulation based on a speed.
	/// </summary>
	public class MriEnterer : MonoBehaviour
	{
		public static float minimalDistanceToEnter = 0.85f;

		[Header("Enter MRI Channel")]
		// Note that I first thought of using a VariableSO for this. However, the process is not instantaneous. Thus, it makes more sense to have a void event, and if needed another var that contains the current state.
		[SerializeField] private BoolVariable _isInsideMRI; //change to false if entering or leaving MRI and true if stop moving when inside MRI

		[Header("Settings")]
		[SerializeField] private Transform _outsidePosition; // position when participant is outside - can be used without tracker (virtual position)
		[SerializeField] private Transform _insidePosition; // position when participant is inside - can be used without tracker (virtual position)
		[SerializeField] IntReference _avatarGenderReference;

		[Header("Camera settings")]
		[SerializeField] private Vector3 _endAngle; // using for no marker movement
		[SerializeField] private Transform _cameraTransformMale;
		[SerializeField] private Transform _cameraTransformFemale;
		private Quaternion _startQuaternion;
		private Quaternion _endQuaternion;
		[SerializeField] private Transform _playerTransform;
		[SerializeField] private ParentConstraint _playerConstraint;
		// [SerializeField] private Transform _trackersParentTransform;

		[Header("Tracker")]
		[SerializeField] private Transform _hipTransform;

		private bool _isMoving = false;
		private bool _isRotationDone = false;
		private bool _isEntering = true;
		private Vector3 _initialPlayerPosition;
		private Vector3 _initialMarkerPosition;
		private Transform _cameraTransform;
		private bool _oldIsEntering; //record if the previous movement was entering or leaving. True if enter last, false if leave last.

		protected void OnEnable()
		{
			_insidePosition.position = _outsidePosition.position + Vector3.right * minimalDistanceToEnter;
		}

		protected void Start()
		{
			ChangeCamera(_avatarGenderReference.Value);
			_oldIsEntering = false; // suppose that the first thing you do is enter
		}

		public void Update()
		{
			if (_isMoving)
			{
				MakeMovement();
			}
		}

		/// <summary>
		/// Call to enter or exit the MRI.
		/// </summary>
		/*		public void EnterMri()
				{
					bool enter = !_isInsideMRI.Value;
					if (_isMoving)
					{
						if (enter == _isEntering)
						{
							return;
						}
						else
						{
							Debug.LogWarning("EnterMri was called when already in movement. Can produce strange behavior.");
						}
					}

					_timeStart = Time.time;
					_isEntering = enter;
					_initialPlayerPosition = _playerTransform.position;
					_initialMarkerPosition = _markerTransformAnchor.value.position;
					// _trackersParentTransform.SetParent(null);

					_startQuaternion = _cameraTransform.rotation;

					_endQuaternion = enter ? _insidePosition.rotation : _outsidePosition.rotation;

					// Save offset
					//_playerConstraint.SetTranslationOffset(0, new Vector3(_playerConstraint.transform.position.x - _playerConstraint.GetSource(0).sourceTransform.position.x, 0.12f, 0));
					_playerConstraint.constraintActive = true;

					_isMoving = true;

					_isInsideMRI.SetValue(enter);
				}
		*/
		public void EnterOrLeaveOrPauseMri()
		{
			bool enter = !_isInsideMRI.Value;
			if (_isMoving)
			{
				if (enter == _isEntering)
				{
					return;
				}
				else
				{
					Debug.LogWarning("EnterMri was called when already in movement. Can produce strange behavior.");
				}
			}

			if (enter)
			{
				_isEntering = enter;
				_initialPlayerPosition = _playerTransform.position;
				_initialMarkerPosition = _hipTransform.position;
				// _trackersParentTransform.SetParent(null);

				_startQuaternion = _cameraTransform.rotation;
				_endQuaternion = !_oldIsEntering ? _insidePosition.rotation : _outsidePosition.rotation; //TO TRANSFORM in if we go to X it's outside to inside and to -X it's inside to outside

				// Save offset
				//_playerConstraint.SetTranslationOffset(0, new Vector3(_playerConstraint.transform.position.x - _playerConstraint.GetSource(0).sourceTransform.position.x, 0.12f, 0));
				_playerConstraint.constraintActive = true;

				_isMoving = true;

				_oldIsEntering = !_oldIsEntering;
			}
			else
			{
				_playerConstraint.constraintActive = false;
				_isMoving = false;
			}

			_isInsideMRI.SetValue(enter);
		}



		/// <summary>
		/// Move MRI room with a marker positionnate on the participant.
		/// </summary>
		private void MakeMovement()
		{
			if (_hipTransform == null)
			{
				Debug.LogError($"No hip transform set in {this}");
				return;
			}

			float t = Vector3.Distance(_initialMarkerPosition, _hipTransform.position) / minimalDistanceToEnter;

			if (!_isRotationDone)
			{
				_cameraTransform.rotation = Quaternion.Slerp(_startQuaternion, _endQuaternion, Easing.QuadraticEaseInOut(t));
			}

			if (t >= 1)
			{
				_isMoving = false;
				_isRotationDone = true;
				// _trackersParentTransform.SetParent(_playerTransform);
			}
		}

		public void ChangeCamera(int genderReference)
		{

			switch (genderReference)
			{
				case 0:
					_cameraTransform = _cameraTransformMale;
					break;
				case 1:
					_cameraTransform = _cameraTransformFemale;
					break;
				default:
					throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Method that make the participant enter the MRI instantaneously.
		/// </summary>
		// [Button]
		public void InstantaneousEnter()
		{
			//_playerTransform.position = _insidePosition.position;
			_playerConstraint.constraintActive = true; //The hip need to be visible and tracked by the motion capture system
			_cameraTransform.rotation = _insidePosition.rotation;
			_isRotationDone = true;
			_isInsideMRI.SetValue(true);
		}
		IEnumerator Wait()
		{
			yield return new WaitForSeconds(2);
			_playerConstraint.constraintActive = false;
		}


		#region DEBUG

		// [ConsoleMethod("mri.change-distance-to-enter", "Change the minimal distance to enter the MRI.")]
		public static void ChangeDistanceToEnter(float newValue)
		{
			minimalDistanceToEnter = newValue;
		}

		#endregion
	}
}
