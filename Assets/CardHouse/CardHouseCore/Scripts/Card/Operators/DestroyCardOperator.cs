using UnityEngine;

namespace CardHouse
{
    public class DestroyCardOperator : MonoBehaviour
    {
        public void Activate()
        {
            var card = GetComponentInParent<Card>();
            var destroyGroup = card.GetDestroyGroup();
            if (destroyGroup != null)
            {
                destroyGroup.Mount(card);
            }
        }
    }
}
