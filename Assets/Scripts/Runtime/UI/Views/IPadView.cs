using PlazmaGames.UI;
using System.Collections;
using System.Collections.Generic;
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

        //[Header("Patients")]
        [SerializeField] private List<Button> _patientEkgs = new List<Button>();
        [SerializeField] private Button _zoomedEkg;

        //[Header("Map")]

        //[Header("Guide")]


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
            _patientView.gameObject.SetActive(true);
            _mapView.gameObject.SetActive(false);
            _guideView.gameObject.SetActive(false);
        }

        private void GotoMap()
        {
            _patientView.gameObject.SetActive(false);
            _mapView.gameObject.SetActive(true);
            _guideView.gameObject.SetActive(false);
        }

        private void GotoGuide()
        {
            _patientView.gameObject.SetActive(false);
            _mapView.gameObject.SetActive(false);
            _guideView.gameObject.SetActive(true);
        }

        public override void Init()
        {
            CancelZoom();

            GotoPatient();

            _patientTab.onClick.AddListener(GotoPatient);
            _mapTab.onClick.AddListener(GotoMap);
            _guideTab.onClick.AddListener(GotoGuide);

            foreach (Button b in _patientEkgs)
            {
                b.onClick.AddListener(() => Zoom(b.GetComponent<RawImage>()));
            }

            _zoomedEkg.onClick.AddListener(CancelZoom);
        }
    }

}
