using PlazmaGames.Attribute;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBlack
{
    public class Cart : MonoBehaviour
    {
        [SerializeField] private LayerMask _itemLayer;
        [SerializeField, ReadOnly] private List<Rigidbody> _items = new();

        private void OnCollisionEnter(Collision collision)
        {
            if ((_itemLayer.value & (1 << collision.gameObject.layer)) > 0)
            {
               collision.rigidbody.isKinematic = true;
               collision.rigidbody.transform.parent = transform;
                if (!_items.Contains(collision.rigidbody)) _items.Add(collision.rigidbody);
            }
        }
    }
}
