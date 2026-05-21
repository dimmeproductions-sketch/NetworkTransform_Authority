### Herramienta de diagnóstico utilizada
Para recrear de forma precisa los escenarios de latencia alta y pérdida de paquetes, se ha utilizado el paquete **Unity Multiplayer Tools**, concretamente la ventana de **Network Simulator**, que ha tenido que ser añadido a la escena como un objeto vacío para realizar las pruebas. Este paquete permite emular las condiciones de una red WAN inestable directamente desde el editor de Unity. Se ha hecho una tabla estimada en base a los presets de conexión ya existentes en el componente y los picos de lag se han probado manualmente con el mismo.

### Tabla comparativa de rendimiento

| Modo de autoridad | Respuesta al input | Impacto de la latencia alta | Impacto de la pérdida de paquetes |
| :--- | :--- | :--- | :--- |
| **0. Server Auth** | Mala (Sufre *input lag*) | Muy alto (Injugable) | Muy alto (Movimiento a saltos) |
| **1. Server + Rewind** | Excelente (Inmediata) | Medio (Provoca *rubberbanding*) | Medio (Correcciones visuales) |
| **2. Client Auth** | Excelente (Inmediata) | Bajo (Solo afecta a los demás) | Bajo para el dueño / Alto para el resto |