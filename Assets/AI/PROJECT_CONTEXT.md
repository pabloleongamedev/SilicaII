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
- Perspective: `PlayerPerspectiveController` alterna primera/tercera persona desde `PlayerInputHandler`; el cambio solo ajusta la camara compartida, no activa/desactiva el cuerpo. En `Pablo_TestMechanics.unity`, `PlayerRobotVisual` permanece visible y `Main Camera` se referencia en `Shared Camera Transform`; `PlayerAnimatorPresenter` sincroniza el Animator con `MovementController`.
- Camera motion: `PlayerCameraMotion` vive en `Main Camera`, lee `MovementController` por Inspector y se sincroniza con `PlayerFootstepAnimationEvents.FootstepReceived`. Cada `AnimationEvent OnFootstep` dispara un pulso local suave de camara si hay input de movimiento, grounded y jetpack inactivo. `stepDuration` define la duracion del pulso y `smoothTime` suaviza la transicion. No controla input, yaw/pitch ni offsets de perspectiva; `PlayerPerspectiveController` conserva el offset base de primera/tercera persona.
- Player visual animation: `PlayerAnimatorPresenter` usa `MovementController.GetVerticalSpeed()` para activar `Jump` por velocidad vertical positiva real y no esperar a que termine `groundedGraceTime`; el umbral es constante interna para no agregar ruido al Inspector. `FreeFall` solo se activa con caida real: no grounded, velocidad vertical negativa suficiente y sin jetpack efectivo. Durante jetpack efectivo, el Animator vuelve a locomocion idle y `Visual Tilt Root` inclina el visual 20 grados hacia el movimiento, con 10 grados extra mientras sprint/boost esta activo.
- Jetpack: `JetpackAbility` aplica rampa de potencia con `MovementConfig_SO.jetpackRampUpTime`; el empuje vertical y el boost horizontal empiezan suaves y suben hasta la fuerza configurada para evitar impulso inicial brusco.
- Inventory: controller central, vistas separadas y persistencia via participante.
- Crafting/Chemistry: datos + controllers + canales, sin UI como dominio.
- Quest: progreso persistente via `QuestSystem` / `QuestSaveData`.
- Notification/Audio/HUD: presenters y servicios/canales. Los pasos y aterrizajes del Player se disparan solo por `AnimationEvent OnFootstep` / `OnLand` en el Animator visual, recibidos por `PlayerFootstepAnimationEvents` y delegados a `PlayerAudioFeedback`. `PlayerFootstepAnimationEvents` filtra duplicados muy cercanos para evitar rafagas por blending/transiciones del Animator. No hay sonido al iniciar salto. Audio de locomocion usa `WalkBase` / `JumpBase` por defecto, y cambia a variantes Grass/Metal si el ground tag contiene `Grass` o `Metal`. El jetpack es la excepcion: su audio lo controla `MovementController.OnJetpackActiveChanged`.
- Interaction/Scanner/World/Delivery: contexto de escena, referencias explicitas, warning + no-op si falta dependencia.

## Escenas y assets clave

- `Assets/Project/Scenes/Pablo_TestMechanics.unity`
- `Assets/Project/Scenes/Menu.unity`
- Menu scene wiring: este es el estado canonico que se debe preservar. `Canvas` contiene `MainMenuManager`, `SceneLoadService` y `SaveLoadSceneBinding`. `MainMenuManager` usa `SceneLoadService` por Inspector y consume `SaveLoadSceneBinding` como `ISaveSlotReader`/`IGameSessionLoader`.
  - Estado inicial: `MenuButton` activo, `BannerGame` activo, `Play_Panel` oculto, `OptionsPanel` oculto y `CreditsPanel` oculto.
  - `Jugar`: no oculta `MenuButton` ni `BannerGame`; solo muestra `Play_Panel` y refresca sus `SaveSlot`.
  - `Opciones`: mantiene `MenuButton`, oculta `BannerGame`, oculta `Play_Panel`, muestra `OptionsPanel` y al cerrar vuelve a `ShowMainMenu`.
  - `Creditos`: mantiene `MenuButton`, oculta `BannerGame`, oculta `Play_Panel`, muestra `CreditsPanel` y al cerrar vuelve a `ShowMainMenu`. El `Button` de creditos debe ejecutar `ShowCredits` y tambien tener wiring explicito `BannerGame.SetActive(false)` / `CreditsPanel.SetActive(true)`; el cierre debe ejecutar `ShowMainMenu` y `BannerGame.SetActive(true)` / `CreditsPanel.SetActive(false)`.
  - `SaveSlot` soporta modo `NewGame` y `LoadGame` para separar crear partida de cargar existente.
- `Assets/Project/Prefabs/UI/OptionsPanel.prefab`
- `Assets/Project/ScriptableObjects/Menu/GameSettings.asset`
- `Assets/PlayerTest/Prefabs/PlayerRobot.prefab` debe mantenerse como prefab visual-only: sin input, movimiento, inventario, scanner, camara, audio listener ni servicios propios.

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
