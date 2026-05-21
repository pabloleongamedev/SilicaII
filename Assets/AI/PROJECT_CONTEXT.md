# SilicaII - Project Context

Este archivo es el punto de arranque para cualquier nueva ventana o sesion de trabajo con IA.
Antes de auditar, modificar o documentar el proyecto, leer este archivo y luego las fuentes de verdad indicadas abajo.

## Objetivo arquitectonico

SilicaII usa una arquitectura Unity modular, desacoplada y orientada a sistemas de gameplay.

La escena compone dependencias mediante referencias visibles por Inspector.
Los sistemas se comunican mediante interfaces pequenas, facades/runtime controllers y ScriptableObject EventChannels.
La persistencia se resuelve con `SaveLoadSceneBinding`, `SaveParticipantRegistry` e `ISaveParticipant`.

## Fuentes de verdad

Leer estos documentos cuando haga falta recuperar contexto completo:

- `Assets/AI/Documentacion/Arquitectura_Tecnica_SilicaII.docx`
- `Assets/AI/Documentacion/Manual_Integracion_SilicaII.docx`
- `Assets/AI/Documentacion/Base/Requerimientos_Sistemas_SilicaII.docx`
- `Assets/AI/Documentacion/Base/Instructivo_Construccion_Sistemas_SilicaII.docx`

Los diagramas asociados viven en:

- `Assets/AI/Documentacion/_diagramas_generados`
- `Assets/AI/Documentacion/Base/_diagramas_base`

## Directriz de mantenimiento documental

Siempre que se cambie arquitectura, dependencias por Inspector, flujos SaveLoad, EventChannels, settings, pausa, UI/HUD, audio, quest, inventory, crafting, scanner, world o cualquier sistema base:

1. Actualizar el codigo/escenas/prefabs necesarios.
2. Ejecutar validaciones razonables.
3. Actualizar los documentos fuente de verdad afectados.
4. Si cambia una relacion visual importante, actualizar tambien el diagrama correspondiente.
5. Dejar claro en la respuesta final que documentos fueron actualizados.

No cerrar una tarea de arquitectura como terminada si los documentos fuente de verdad quedaron desalineados con el codigo.

## Reglas duras

- No reintroducir `GameManager.Instance`.
- No reintroducir `GameSettings.Instance`.
- No reintroducir buses estaticos legacy.
- No reintroducir rutas globales mutables como `activeBox`.
- No usar `FindObjectOfType`, `FindFirstObjectByType`, `GameObject.Find` o `Resources.LoadAll` como solucion de arquitectura.
- No usar `AddComponent` automatico para tapar dependencias faltantes.
- No modificar `Time.timeScale` desde UI concreta; usar `IGamePauseService` / `GamePauseService`.
- Las dependencias de escena deben ser visibles por Inspector.
- Si falta una referencia critica, emitir warning claro y hacer no-op controlado.
- UI y audio deben consumir interfaces, servicios asignados o canales, no implementaciones globales.
- SaveLoad debe persistir por `ISaveParticipant` y `SaveParticipantRegistry`.

## Capas esperadas por sistema

Estructura recomendada:

```text
System/
  Data/
  Core/
  Runtime/
  UI/
  Events/
  Debug/
```

Reglas de capas:

- `Data`: ScriptableObjects, DTOs y configuracion editable.
- `Core`: contratos, modelos y reglas de dominio.
- `Runtime`: MonoBehaviours, services, facades, controllers y scene bindings.
- `UI`: presenters, views y componentes visuales.
- `Events`: EventChannels ScriptableObject y payloads.
- `Debug`: herramientas no obligatorias para runtime.

## Sistemas principales

- Events: `EventChannel_SO` por dominio, asignado por Inspector.
- SaveLoad: `SaveLoadSceneBinding` como fachada de escena, `SaveParticipantRegistry`, `ISaveParticipant`, `GameData`.
- Settings: `GameSettings.asset` + `GameSettingsService`, consumido por `IGameSettingsReader` / `IGameSettingsWriter`.
- Pause: `IGamePauseService` + `GamePauseService`.
- Player/Input: `PlayerInputHandler` depende de referencias serializadas.
- Perspective: `PlayerPerspectiveController` alterna primera/tercera persona desde `PlayerInputHandler`; la tercera persona es presentacion visual/camara, no un segundo movimiento/input.
- Inventory: controller central, vistas separadas y persistencia via participante.
- Crafting/Chemistry: datos + controllers + canales, sin UI como dominio.
- Quest: progreso persistente via `QuestSystem` / `QuestSaveData`.
- Notification/Audio/HUD: presenters y servicios/canales.
- Interaction/Scanner/World/Delivery: contexto de escena, referencias explicitas, warning + no-op si falta dependencia.

## Escenas y assets clave

- `Assets/Project/Scenes/Pablo_TestMechanics.unity`
- `Assets/Project/Scenes/Menu.unity`
- `Assets/Project/Prefabs/UI/OptionsPanel.prefab`
- `Assets/Project/ScriptableObjects/Menu/GameSettings.asset`

## Validaciones recomendadas

Health check completo:

```powershell
powershell -ExecutionPolicy Bypass -File .\Assets\Project\Scripts\AI\Validation\RunProjectHealthCheck.ps1
```

Health check incluyendo errores recientes del log de Unity:

```powershell
powershell -ExecutionPolicy Bypass -File .\Assets\Project\Scripts\AI\Validation\RunProjectHealthCheck.ps1 -ShowUnityLogErrors
```

Compilacion individual:

```powershell
dotnet build .\SilicaII.Systems.csproj
```

Reglas de arquitectura:

```powershell
powershell -ExecutionPolicy Bypass -File .\Assets\Project\Scripts\AI\Validation\RunArchitectureChecks.ps1
```

Busquedas utiles:

```powershell
rg -n "GameManager\.Instance|GameSettings\.Instance|AudioManager\.Instance" Assets/Project/Scripts
rg -n "GameplayEvents\.|QuestEvents\.|InventoryEvents\.|CraftingEvents\.|NotificationEvents\." Assets/Project/Scripts
rg -n "FindObjectOfType|FindFirstObjectByType|GameObject\.Find|Resources\.LoadAll" Assets/Project/Scripts
rg -n "activeBox|Time\.timeScale" Assets/Project/Scripts
```

Validacion manual en Unity:

- Abrir `Pablo_TestMechanics.unity`, `Menu.unity` y `OptionsPanel.prefab`.
- Confirmar que no haya Missing Script.
- Probar menu: nueva partida y cargar.
- Probar gameplay: checkpoint save/restore, autosave si aplica, inventory, crafting, quest, scanner, delivery, world.
- Probar perspectiva: tecla `C` / accion `Player.Camera` alterna primera y tercera persona si `PlayerPerspectiveController` esta configurado por Inspector.
- Probar opciones: brillo, fullscreen y volumen.
- Probar pausa: abrir, reanudar y volver a menu.

## Como iniciar una nueva ventana con IA

Usar este prompt:

```text
Antes de responder, lee Assets/AI/PROJECT_CONTEXT.md.
Despues usa como fuentes de verdad:
- Assets/AI/Documentacion/Arquitectura_Tecnica_SilicaII.docx
- Assets/AI/Documentacion/Manual_Integracion_SilicaII.docx
- Assets/AI/Documentacion/Base/Requerimientos_Sistemas_SilicaII.docx
- Assets/AI/Documentacion/Base/Instructivo_Construccion_Sistemas_SilicaII.docx

Respeta las reglas del proyecto y, si haces cambios arquitectonicos, actualiza tambien los documentos fuente de verdad.
```

## Nota de versionado

Si este archivo queda ignorado por `.gitignore`, revisar la regla `*.md`.
Este contexto conviene versionarlo o mantenerlo sincronizado manualmente con los documentos fuente de verdad.
