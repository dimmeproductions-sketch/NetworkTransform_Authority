using Unity.Netcode;
using UnityEngine;

namespace Code
{
    public class CodePlayer : NetworkBehaviour
    {
        public float speed = 5f;
        private float m_VerticalVelocity = 0f;
        private Vector2 m_ServerInput;
        private bool m_JumpRequested;
        private Vector2 m_LastInput;
        public float mapLimitX = 5f; // Límite en el eje X (izquierda/derecha)
        public float mapLimitZ = 5f; // Límite en el eje Z (arriba/abajo)

        [Rpc(SendTo.Server)]
        private void SendInputServerRpc(Vector2 input, bool jump)
        {
            m_ServerInput = input;
            if (jump) m_JumpRequested = true;
        }

        private void Update()
        {
            if (IsOwner)
            {
                Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                bool jump = Input.GetButtonDown("Jump");

                if (IsServer) // Si somos el Host, lo asignamos directamente sin pasar por la red
                {
                    m_ServerInput = input;
                    if (jump) m_JumpRequested = true;
                }
                // Si somos cliente, solo enviamos el RPC si cambió el movimiento o si queremos saltar
                else if (input != m_LastInput || jump)
                {
                    SendInputServerRpc(input, jump);
                    m_LastInput = input;
                }
            }

            if (IsServer)
            {
                // Movimiento horizontal en el plano XZ (Arriba, Abajo, Izquierda, Derecha)
                Vector3 moveDir = new Vector3(m_ServerInput.x, 0, m_ServerInput.y).normalized;
                transform.Translate(moveDir * (speed * Time.deltaTime), Space.World);

                // Impedir la salida del plano XZ
                float clampedX = Mathf.Clamp(transform.position.x, -mapLimitX, mapLimitX);
                float clampedZ = Mathf.Clamp(transform.position.z, -mapLimitZ, mapLimitZ);
                transform.position = new Vector3(clampedX, transform.position.y, clampedZ);

                // Lógica de Salto y Gravedad (Asumiendo suelo base en Y = 1f)
                if (transform.position.y > 1f || m_JumpRequested)
                {
                    // Si pide saltar y está tocando el suelo de forma aproximada
                    if (m_JumpRequested && transform.position.y <= 1.05f)
                    {
                        m_VerticalVelocity = 5f; // Fuerza del salto
                        m_JumpRequested = false;
                    }

                    // Aplicamos gravedad simulada de Unity
                    m_VerticalVelocity += Physics.gravity.y * Time.deltaTime;
                    transform.Translate(Vector3.up * (m_VerticalVelocity * Time.deltaTime), Space.World);

                    // Corrección de caída para no atravesar el suelo plano (Y = 1f)
                    if (transform.position.y < 1f)
                    {
                        transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
                        m_VerticalVelocity = 0f;
                    }
                }
                else
                {
                    m_JumpRequested = false; // Evita acumular saltos fantasma en el suelo
                }
            }
        }
    }
}