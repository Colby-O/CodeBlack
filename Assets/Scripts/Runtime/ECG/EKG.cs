using UnityEngine;
using System.Collections;

namespace CodeBlack.ECG
{
    public class EKG : MonoBehaviour
    {
        [SerializeField] private Heart _heart;

        [SerializeField] private float _traceHeight = 0.25f;
        [SerializeField] private int _traceWidth = 5;       
        private float _traceWidthHalved;
        [SerializeField] private float _traceLineWidth = 0.005f;    
        private float _tracePlotInterval;     
        private float _traceHeightOffset;     

        private float _traceVoltage = 0.0f;     
        private float _traceLastVoltage = 0.0f;    

        private Mesh _mesh;         
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private int _traceQuadCount;       
        private int _traceVerticesCount;      
        private int _traceTriangleCount;      
        private int _traceNormalCount;      
        private int _traceQuadCounter = 0;     
        private Vector3[] _traceVertices;     
        private Vector3[] _traceNormals;     
        private int[] _traceTriangles;      
        private Color[] _traceColors;

        [SerializeField] private Color _goodColor;
        [SerializeField] private Color _badColor;

        private float _traceColorRed = 0.0f;
        private float _traceColorGreen = 1.0f;
        private float _traceColorBlue = 0.0f;     

        private float _vectorPX;       
        private float _vectorPY;       
        private float _vectorLength;      
        private float _vectorNX;        
        private float _vectorNY;        
        private float _P1X;         
        private float _P2X;         
        private float _P3X;        
        private float _P4X;        
        private float _P1Y;        
        private float _P2Y;         
        private float _P3Y;         
        private float _P4Y;        


        private int _index;

        void Start()
        {
            _tracePlotInterval = 0.01f;            
            _traceHeightOffset = _traceLineWidth * 0.5f;       
            _traceWidthHalved = _traceWidth * 0.25f;       

            CreateMesh();
            if (_heart == null) _heart = GetComponent<Heart>();
            if (_meshRenderer == null) _meshRenderer = GetComponent<MeshRenderer>();
            _traceQuadCounter = 0;
        }

        private void Update()
        {
            if (_heart.IsHealthty())
            {
                _traceColorRed = _goodColor.r;
                _traceColorBlue = _goodColor.b;
                _traceColorGreen = _goodColor.g;

                _meshRenderer.material.color = _goodColor;
            }
            else
            {
                _traceColorRed = _badColor.r;
                _traceColorBlue = _badColor.b;
                _traceColorGreen = _badColor.g;

                _meshRenderer.material.color = _badColor;
            }
        }

        void FixedUpdate()
        {
            GetTraceValues();
            UpdateMesh();
        }

        public void SetHeart(Heart heart)
        {
            _heart = heart;
        }

        void CreateMesh()
        {
            _mesh = new Mesh();
            _meshFilter = GetComponent<MeshFilter>();
            _meshFilter.mesh = _mesh;

            _traceQuadCount = 50 * _traceWidth;     
            _traceTriangleCount = _traceQuadCount * 6;   
            _traceVerticesCount = _traceQuadCount * 4;    

            _traceVertices = new Vector3[_traceVerticesCount]; 
            _traceNormals = new Vector3[_traceVerticesCount]; 
            _traceTriangles = new int[_traceTriangleCount];  
            _traceColors = new Color[_traceVerticesCount];  

            _index = 0;

            for (int n = 0; n < _traceQuadCount; n++)
            {
                _traceVertices[_index] = new Vector3((_tracePlotInterval * n) - _traceWidthHalved, -_traceHeightOffset, 0.0f);    
                _traceColors[_index] = new Color(_traceColorRed, _traceColorGreen, _traceColorBlue, 1);         
                _traceVertices[_index + 1] = new Vector3((_tracePlotInterval * n) - _traceWidthHalved, _traceHeightOffset, 0.0f);   
                _traceColors[_index + 1] = new Color(_traceColorRed, _traceColorGreen, _traceColorBlue, 1);        
                _traceVertices[_index + 2] = new Vector3((_tracePlotInterval * (n + 1)) - _traceWidthHalved, -_traceHeightOffset, 0.0f);  
                _traceColors[_index + 2] = new Color(_traceColorRed, _traceColorGreen, _traceColorBlue, 1);        
                _traceVertices[_index + 3] = new Vector3((_tracePlotInterval * (n + 1)) - _traceWidthHalved, _traceHeightOffset, 0.0f);  
                _traceColors[_index + 3] = new Color(_traceColorRed, _traceColorGreen, _traceColorBlue, 1);         
                _index += 4;                           
            }

            _index = 0;

            for (int n = 0; n < _traceQuadCount; n++)   
            {
                _traceTriangles[_index] = n * 4;     
                _traceTriangles[_index + 1] = (n * 4) + 1;  
                _traceTriangles[_index + 2] = (n * 4) + 2;  
                _traceTriangles[_index + 3] = (n * 4) + 2;  
                _traceTriangles[_index + 4] = (n * 4) + 1; 
                _traceTriangles[_index + 5] = (n * 4) + 3; 

                _index += 6;
            }

            _mesh.vertices = _traceVertices;
            _mesh.normals = _traceNormals;
            _mesh.triangles = _traceTriangles;
            _mesh.colors = _traceColors;
        }


        void GetTraceValues()
        {

            Vector2 traceVoltages = _heart.GetVoltage();
            _traceVoltage = traceVoltages.x;
            _traceLastVoltage = traceVoltages.y;

            _traceVoltage *= 0.1f * _traceHeight;
            _traceLastVoltage *= 0.1f * _traceHeight;
        }

        void UpdateMesh()   
        {
            _traceVertices = _mesh.vertices;               
            _traceColors = _mesh.colors;               


            _vectorPX = _traceVoltage - _traceLastVoltage;            
            _vectorPY = -_tracePlotInterval;               
            _vectorLength = Mathf.Sqrt((_vectorPX * _vectorPX) + (_vectorPY * _vectorPY));    
            _vectorNX = _vectorPX / _vectorLength;             
            _vectorNY = _vectorPY / _vectorLength;              

            _P1X = (_tracePlotInterval * _traceQuadCounter) + (_vectorNX * _traceHeightOffset) - _traceWidthHalved;  
            _P1Y = (_traceLastVoltage) + (_vectorNY * _traceHeightOffset);           
            _P2X = (_tracePlotInterval * _traceQuadCounter) - (_vectorNX * _traceHeightOffset) - _traceWidthHalved;  
            _P2Y = (_traceLastVoltage) - (_vectorNY * _traceHeightOffset);           
            _P3X = (_tracePlotInterval * (_traceQuadCounter + 1)) + (_vectorNX * _traceHeightOffset) - _traceWidthHalved;  
            _P3Y = (_traceVoltage) + (_vectorNY * _traceHeightOffset);              
            _P4X = (_tracePlotInterval * (_traceQuadCounter + 1)) - (_vectorNX * _traceHeightOffset) - _traceWidthHalved;  
            _P4Y = (_traceVoltage) - (_vectorNY * _traceHeightOffset);

            _traceVertices[_traceQuadCounter * 4] = new Vector3(_P1X, _P1Y, 0.0f);      
            _traceVertices[_traceQuadCounter * 4 + 1] = new Vector3(_P2X, _P2Y, 0.0f);    
            _traceVertices[_traceQuadCounter * 4 + 2] = new Vector3(_P3X, _P3Y, 0.0f);     
            _traceVertices[_traceQuadCounter * 4 + 3] = new Vector3(_P4X, _P4Y, 0.0f);    
            _traceColors[_traceQuadCounter * 4] = new Color(1, 1, 1, 1);        
            _traceColors[_traceQuadCounter * 4 + 1] = new Color(1, 1, 1, 1);      
            _traceColors[_traceQuadCounter * 4 + 2] = new Color(1, 1, 1, 1);       
            _traceColors[_traceQuadCounter * 4 + 3] = new Color(1, 1, 1, 1);       


            if (_traceQuadCounter - 1 < 0)               
            {
                _traceColors[(_traceQuadCount - 1) * 4] = new Color(_traceColorRed, _traceColorGreen, _traceColorBlue, 1);
                _traceColors[((_traceQuadCount - 1) * 4) + 1] = new Color(_traceColorRed, _traceColorGreen, _traceColorBlue, 1);
                _traceColors[((_traceQuadCount - 1) * 4) + 2] = new Color(_traceColorRed, _traceColorGreen, _traceColorBlue, 1);
                _traceColors[((_traceQuadCount - 1) * 4) + 3] = new Color(_traceColorRed, _traceColorGreen, _traceColorBlue, 1);
            }
            else                                                                                 
            {
                _traceColors[(_traceQuadCounter - 1) * 4] = new Color(_traceColorRed, _traceColorGreen, _traceColorBlue, 1);
                _traceColors[(_traceQuadCounter - 1) * 4 + 1] = new Color(_traceColorRed, _traceColorGreen, _traceColorBlue, 1);
                _traceColors[(_traceQuadCounter - 1) * 4 + 2] = new Color(_traceColorRed, _traceColorGreen, _traceColorBlue, 1);
                _traceColors[(_traceQuadCounter - 1) * 4 + 3] = new Color(_traceColorRed, _traceColorGreen, _traceColorBlue, 1);
            }


            _index = _traceQuadCounter + 1;


            for (int i = 0; i < _traceWidth * 10.0f; i++)            
            {
                if (_index + 1 > _traceQuadCount)
                {
                    _index = 0;
                }

                float fadeColor = Mathf.Clamp(i * 0.1f - 1.0f, 0.0f, 1.0f);   

                _traceColors[_index * 4] = new Color(_traceColorRed, _traceColorGreen, _traceColorBlue, fadeColor);
                _traceColors[_index * 4 + 1] = new Color(_traceColorRed, _traceColorGreen, _traceColorBlue, fadeColor);
                _traceColors[_index * 4 + 2] = new Color(_traceColorRed, _traceColorGreen, _traceColorBlue, fadeColor);
                _traceColors[_index * 4 + 3] = new Color(_traceColorRed, _traceColorGreen, _traceColorBlue, fadeColor);

                _index++;
            }

            _traceQuadCounter++;

            if (_traceQuadCounter + 1 > _traceQuadCount)
            {
                _traceQuadCounter = 0;
            }

            _mesh.vertices = _traceVertices;
            _mesh.colors = _traceColors;
        }
    }
}
