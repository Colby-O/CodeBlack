using PlazmaGames.Attribute;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBlack
{
    public class Cart : MonoBehaviour
    {
        [SerializeField] private LayerMask _itemLayer;

        [SerializeField, ReadOnly] private List<Rigidbody> _items;

        private void OnCollisionEnter(Collision collision)
        {
            if ((_itemLayer.value & (1 << collision.gameObject.layer)) > 0)
            {
                if (!_items.Contains(collision.rigidbody)) _items.Add(collision.rigidbody);
                else _items.Remove(collision.rigidbody);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if ((_itemLayer.value & (1 << collision.gameObject.layer)) > 0)
            {
                if (!_items.Contains(collision.rigidbody)) _items.Add(collision.rigidbody);
                else _items.Remove(collision.rigidbody);
            }
        }

        private void Awake()
        {
            _items = new List<Rigidbody> ();
        }

        private void Update()
        {
            foreach (Rigidbody rb in _items)
            {
               rb.isKinematic = true;
               rb.transform.parent = transform;
            }
        }
    }
}
