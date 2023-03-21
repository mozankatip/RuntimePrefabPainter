# Runtime Prefab Painter Documentation
## Introduction
Runtime Prefab Painter is a runtime Unity tool that allows you to paint prefabs onto target objects in a scene using GPU instancing. It can be used for placing objects like trees, rocks, grass, etc., on a terrain or any other object in the scene.
## Features
Runtime Prefab Painter has the following features:
* Customizable brush parameters like paint radius, brush density, and maximum instances per batch.
* Supports multiple prefabs. The user can define a list of prefabs that will be used for painting.
* Supports random rotation of the prefabs based on the surface normal of the object being painted on.
* Supports batching of instances. The tool creates batches of instances and renders them using Graphics.DrawMeshInstanced() for better performance.
* Supports selection of a random prefab from the prefab list to start with.
## Usage
To use the Painter Tool, follow these steps:
1. Create an empty GameObject in the scene and attach the Painter script to it.
2. Create a new layer in the Layer Manager and assign it to the objects you want to paint on.
3. Add mesh collider to target objects.
4. Drag and drop the prefabs you want to use for painting onto the Prefab List parameter of the Painter script.
5. Make sure “Enable GPU Instancing” is ticked on materials of chosen Prefabs.
6. Click and hold the left mouse button to paint instances on the selected object(s).
7. Release the left mouse button to stop painting.

## Parameters
The following parameters can be set in the Inspector window of the Painter script:

| Parameter | Description|
| ------------- | ------------- |
| **Paint Target Layer**  | The layer(s) on which the painting will be done.  |
| **Prefab List**   | A list of GameObjects that will be used as the paint prefabs.  |
| **Paint Target Layer**  | The layer(s) on which the painting will be done.  |
| **Paint Radius**  | The radius of the brush.  |
|**Brush Density**  | The number of instances to be painted per click.  |
| **Max Instances In Batch**  | The maximum number of instances that can be rendered per batch. Each batch should not exceed the limit of 1023.  |
| **Max Batch Count**  |The maximum number of batches that can be created.  |
| **Start Height**  |The height at which the instances will be created.  |

## Notes
* The Painter Tool uses Raycasting to determine the object(s) on which the painting will be done. Make sure the object(s) are on the layer(s) specified in the Paint Target Layer parameter.
* The tool creates batches of instances for better performance. If the number of instances is higher than the Max Instances In Batch parameter, the tool will create a new batch.
* The total number of instances that can be rendered using Graphics.DrawMeshInstanced() is limited to 100,000. The tool will stop creating new batches if the limit is exceeded.
* The tool supports random rotation of the instances based on the surface normal of the object being painted on. If you don't want this behavior, remove the code that calculates the instance rotation in the CreateInstance() method.
* The tool selects a random prefab from the Prefab List to start with. If you want to start with a specific prefab, change the code in the Start() method accordingly.

