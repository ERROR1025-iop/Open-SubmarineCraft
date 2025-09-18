using UnityEngine;


namespace Scraft
{
    public class AssemblerPCInput : MonoBehaviour
    {



        void Start()
        {

        }


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                AttributeSelector.instance.onCancelSelectButtonClick();
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                AttributeSelector.instance.onDeleteButtonClick();
            }

            moveSelectGameObjects();
        }

        void moveSelectGameObjects()
        {
            float moveSpeed = 0.001f;

            if (Input.GetKey(KeyCode.UpArrow))
            {
                moveSelectGameObjects(Vector3.up * moveSpeed);
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                moveSelectGameObjects(-Vector3.up * moveSpeed);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                moveSelectGameObjects(Vector3.right * moveSpeed);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                moveSelectGameObjects(-Vector3.right * moveSpeed);
            }

            if (Input.GetKey(KeyCode.PageUp))
            {
                moveSelectGameObjects(Vector3.forward * moveSpeed);
            }

            if (Input.GetKey(KeyCode.PageDown))
            {
                moveSelectGameObjects(-Vector3.forward * moveSpeed);
            }
        }

        void moveSelectGameObjects(Vector3 vector)
        {
            GameObject[] selection = IRT.Selection.gameObjects;
            if (selection == null)
            {
                return;
            }

            int count = selection.Length;
            for (int i = 0; i < count; i++)
            {
                selection[i].transform.Translate(vector);
            }

            IRT.Selection.activeObject = null;
            IRT.Undo.Undo();
        }
    }
}