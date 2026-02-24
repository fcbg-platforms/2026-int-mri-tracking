namespace CorriDoor.CameraSystemManagement
{
	using System;
	using UnityAtoms.BaseAtoms;
	using UnityEngine;

	/// <summary>
	/// Spawns the correct camera rig based on the selected system and avatar gender.
	/// </summary>
	public class CameraSpawner : MonoBehaviour
	{
		[Header("System Prefabs")]
		[SerializeField] private Transform _desktopPrefab;
		[SerializeField] private Transform _vrPrefab;
		[SerializeField] private Transform _mriPrefab;

		[Header("SOs References")]
		[SerializeField] private CameraSystemChoiceSO _systemChoiceSO;
		[SerializeField] private IntReference _avatarGenderReference;
		[SerializeField] private Transform _maleHeadTransform;
		[SerializeField] private Transform _femaleHeadTransform;

		private Transform _playerTransform;

		#region Unity functions

		/// <summary>
		/// Unity callback invoked when the component becomes enabled.
		/// </summary>
		protected void OnEnable()
		{
			SpawnPlayerPrefab();
		}
		#endregion

		/// <summary>
		/// Instantiates the proper camera system prefab and parents it to the correct head anchor.
		/// </summary>
		void SpawnPlayerPrefab()
		{
			Debug.Log("Spawn Prefab");
			if (_playerTransform != null)
			{
				Destroy(_playerTransform);
			}
			// Choose the head anchor based on the avatar gender reference.
			Transform headAnchorTransform = _avatarGenderReference.Value == 0 ? _maleHeadTransform : _femaleHeadTransform;

			switch (_systemChoiceSO.Value)
			{
				case CameraSystem.Desktop:
					_playerTransform = Instantiate(_desktopPrefab, headAnchorTransform.position, headAnchorTransform.rotation, headAnchorTransform);
					break;
				case CameraSystem.VR:
					_playerTransform = Instantiate(_vrPrefab);
					break;
				case CameraSystem.MRI:
					_playerTransform = Instantiate(_mriPrefab, headAnchorTransform.position, headAnchorTransform.rotation, headAnchorTransform);
					break;
				default:
					throw new NotImplementedException();
			}
		}
	}
}
