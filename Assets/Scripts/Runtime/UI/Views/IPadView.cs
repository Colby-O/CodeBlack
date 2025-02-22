using System;
using CodeBlack;
using PlazmaGames.UI;
using System.Collections;
using System.Collections.Generic;
using PlazmaGames.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBack.UI { 

	public class IPadView : View
	{
		[Header("TabBar")]
		[SerializeField] private GameObject _patientView;
		[SerializeField] private GameObject _mapView;
		[SerializeField] private GameObject _guideView;
		[SerializeField] private Button _patientTab;
		[SerializeField] private Button _mapTab;
		[SerializeField] private Button _guideTab;
		[SerializeField] private TMP_Text _timeText;

		//[Header("Patients")]
		[SerializeField] private List<Button> _patientEkgs = new List<Button>();
		[SerializeField] private Button _zoomedEkg;

		//[Header("Map")]

		[Header("Guide")]
		[SerializeField] private Guide _guide;
		[SerializeField] private Button _back;
		[SerializeField] private Button _next;

		private void Zoom(RawImage patient)
		{
			_zoomedEkg.GetComponent<RawImage>().texture = patient.texture;
			_zoomedEkg.gameObject.SetActive(true);
		}

		private void CancelZoom()
		{
			_zoomedEkg.gameObject.SetActive(false);
		}

		private void GotoPatient()
		{
			CodeBlackGameManager.isPaused = false;
			_patientView.gameObject.SetActive(true);
			_mapView.gameObject.SetActive(false);
			_guideView.gameObject.SetActive(false);
		}

		private void GotoMap()
		{
			CodeBlackGameManager.isPaused = true;
			_patientView.gameObject.SetActive(false);
			_mapView.gameObject.SetActive(true);
			_guideView.gameObject.SetActive(false);
		}

		private void GotoGuide()
		{
			CodeBlackGameManager.isPaused = true;
			_patientView.gameObject.SetActive(false);
			_mapView.gameObject.SetActive(false);
			_guideView.gameObject.SetActive(true);
		}

		public override void Show()
		{
			base.Show();
			Cursor.lockState = CursorLockMode.Confined;
			Cursor.visible = true;
		}

		public override void Hide()
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			GotoPatient();
		}

		private void Next()
		{
			_back.gameObject.SetActive(true);
			if(!_guide.Next())
			{
				_next.gameObject.SetActive(false);
			}
		}

		private void Back()
		{
			_next.gameObject.SetActive(true);
			if (!_guide.Back())
			{
				_back.gameObject.SetActive(false);
			}
		}

		public override void Init()
		{
			CancelZoom();

			GotoPatient();

			_back.gameObject.SetActive(false);

			_patientTab.onClick.AddListener(GotoPatient);
			_mapTab.onClick.AddListener(GotoMap);
			_guideTab.onClick.AddListener(GotoGuide);

			foreach (Button b in _patientEkgs)
			{
				b.onClick.AddListener(() => Zoom(b.GetComponent<RawImage>()));
			}

			_next.onClick.AddListener(Next);
			_back.onClick.AddListener(Back);

			_zoomedEkg.onClick.AddListener(CancelZoom);
		}

		private void FixedUpdate()
        {
            DateTime time = new DateTime(2025, 1, 1, 22, 0, 0);
            time = time.AddSeconds(CodeBlackGameManager.RunningTime());
            _timeText.text = time.ToString("HH:mm");
        }
	}

}
