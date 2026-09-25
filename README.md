# MicroJuego Asteroids

Microjuego 2D desarrollado en Unity.

El jugador controla una nave espacial, esquiva meteoritos, dispara proyectiles y destruye asteroides. En la segunda iteración se incorporan la fragmentación de asteroides, la pausa, el reinicio de partida y el patrón Object Pooling.

## Requisitos

- Unity 6
- Windows 11 para ejecutar la demo


## Controles

| Acción | Control |
| --- | --- |
| Girar | Flechas izquierda/derecha o A/D |
| Acelerar | Flecha arriba o W |
| Disparar | Barra espaciadora o clic izquierdo |
| Pausar / reanudar | Escape |
| Reiniciar | Botón de reinicio del menú de pausa |

Los controles de movimiento utilizan los ejes `Rotate` y `Thrust`, con compatibilidad alternativa para los ejes estándar `Horizontal` y `Vertical` de Unity.

## Estructura del repositorio

```text
MicroJuego_v1.3/
├── Proyecto_Unity/
│   ├── Assets/
│   │   ├── Scenes/MainScene.unity
│   │   ├── Prefabs/
│   │   ├── Scripts/
│   │   ├── Sprites/
│   │   └── TextMesh Pro/
│   ├── Packages/
│   └── ProjectSettings/
├── Documentacion/
│   └── gdd_jaimecalero_V1.2.pdf
└── Demo/
    └── Asteroids Microjuego.exe
```

## Scripts principales

- `Player.cs`: movimiento, giro, disparo y screen wrapping.
- `Bullet.cs`: movimiento, vida del proyectil, impactos y puntuación.
- `Asteroid.cs`: movimiento, colisiones y fragmentación angular.
- `EnemySpawner.cs`: generación progresiva de asteroides.
- `ObjectPool.cs`: reutilización de balas y asteroides.
- `GameManager.cs`: pausa, reinicio y estado de Game Over.
- `UIManager.cs`: puntuación y menús de pausa y derrota.

## Demo ejecutable

La carpeta `Demo` contiene una versión compilada para Windows. Para jugar, ejecutar:

Demo/Asteroids Microjuego.exe


## Documentación

El documento de diseño se encuentra en `Documentacion`. En él se describen la escena, los assets, las mecánicas, la arquitectura de scripts, los controles y el sistema de Object Pooling.

## Autor

Jaime Calero
