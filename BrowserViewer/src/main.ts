
import * as OBC from "@thatopen/components";
import * as THREE from "three";

import "./style.css";

// ==========================================
// NEXUS | BIM INTELLIGENCE
// ARCHITECT ARZATH
// ==========================================

// HTML ELEMENTS
const container = document.getElementById("container")!;
const fileInput = document.getElementById("ifcFile") as HTMLInputElement;
const status = document.getElementById("modelStatus")!;
const progress = document.getElementById("progress")!;
const modelInfo = document.getElementById("modelInfo")!;
const resetButton = document.getElementById("resetView") as HTMLButtonElement;
const downloadButton = document.getElementById("downloadFrag") as HTMLButtonElement;

// ==========================================
// INITIALIZE REVIT / IFC VIEWER
// ==========================================

const components = new OBC.Components();

const worlds = components.get(OBC.Worlds);

const world = worlds.create<
  OBC.SimpleScene,
  OBC.OrthoPerspectiveCamera,
  OBC.SimpleRenderer
>();

world.scene = new OBC.SimpleScene(components);
world.scene.setup();
world.scene.three.background = null;

world.renderer = new OBC.SimpleRenderer(components, container);
world.camera = new OBC.OrthoPerspectiveCamera(components);

await world.camera.controls.setLookAt(
  80, 60, 80,
  0, 0, 0
);

components.init();

components.get(OBC.Grids).create(world);

// ==========================================
// IFC LOADER
// ==========================================

const ifcLoader = components.get(OBC.IfcLoader);
const fragments = components.get(OBC.FragmentsManager);

await ifcLoader.setup({
  autoSetWasm: false,
  wasm: {
    path: "https://unpkg.com/web-ifc@0.0.79/",
    absolute: true
  }
});

const workerUrl = await OBC.FragmentsManager.getWorker();
fragments.init(workerUrl);

// ==========================================
// CAMERA AND FRAGMENT SYNCHRONIZATION
// ==========================================

world.camera.controls.addEventListener("update", () => {
  fragments.core.update();
});

let loadedModel: any = null;

// ==========================================
// AUTOMATIC MODEL FRAMING
// ==========================================


async function frameModel(model: any) {
  if (!model) {
    progress.textContent = "No model available.";
    return;
  }

  try {
    fragments.core.update(true);

    // Use the fragment-aware bounding box component
    const boxer = components.get(OBC.BoundingBoxer);

    boxer.list.clear();
    boxer.addFromModels();

    const box = boxer.get();

    boxer.list.clear();

    console.log("NEXUS FRAGMENT BOUNDS:", box);

    if (box.isEmpty()) {
      progress.textContent =
        "Geometry bounds unavailable. Further inspection required.";
      console.error("NEXUS: Fragment bounding box is empty.");
      return;
    }

    const sphere = new THREE.Sphere();
    box.getBoundingSphere(sphere);

    await world.camera.controls.fitToSphere(
      sphere,
      true
    );

    progress.textContent =
      "Model automatically framed successfully.";

    console.log("NEXUS: Model framing successful.");

  } catch (error) {
    console.error("NEXUS FRAMING ERROR:", error);
    progress.textContent = "Model framing failed.";
  }
}

// ==========================================
// FRAGMENT MODEL REGISTRATION
// ==========================================

fragments.list.onItemSet.add(async ({ value: model }) => {
  model.useCamera(world.camera.three);

  world.scene.three.add(model.object);

  fragments.core.update(true);

  console.log("NEXUS: Fragment model registered.");

  // Allow geometry to update before calculating bounds
  await new Promise<void>((resolve) => {
    requestAnimationFrame(() => resolve());
  });

  if (loadedModel === model) {
    await frameModel(model);
  }
});

// ==========================================
// IFC FILE UPLOAD
// ==========================================

fileInput.addEventListener("change", async () => {
  const file = fileInput.files?.[0];

  if (!file) return;

  try {
    status.textContent = "LOADING MODEL";
    progress.textContent = "Reading IFC file...";

    downloadButton.disabled = true;

    const data = new Uint8Array(
      await file.arrayBuffer()
    );

    progress.textContent =
      "Converting IFC to 3D fragments...";

    const modelName = file.name.replace(/\.ifc$/i, "");

    const result = await ifcLoader.load(
      data,
      true,
      modelName,
      {
        processData: {
          progressCallback: (value: number) => {
            progress.textContent =
              `Processing IFC: ${Math.round(value * 100)}%`;
          }
        }
      }
    );

    loadedModel = result;

    status.textContent = "MODEL LOADED";

    progress.textContent =
      "IFC conversion completed. Preparing 3D view...";

    modelInfo.textContent =
      `Model: ${file.name}\n` +
      `Size: ${(file.size / 1048576).toFixed(2)} MB`;

    downloadButton.disabled = false;

    // Ensure the model is registered before framing
    if (fragments.list.get(modelName)) {
      await frameModel(loadedModel);
    }

  } catch (error) {
    console.error("NEXUS IFC ERROR:", error);

    status.textContent = "LOAD FAILED";
    progress.textContent = String(error);
  }
});

// ==========================================
// RESET CAMERA VIEW
// ==========================================

resetButton.addEventListener("click", async () => {
  if (!loadedModel) {
    progress.textContent = "Load an IFC model first.";
    return;
  }

  await frameModel(loadedModel);
});

// ==========================================
// EXPORT FRAGMENTS
// ==========================================

downloadButton.addEventListener("click", async () => {
  if (!loadedModel) return;

  try {
    const buffer = await loadedModel.getBuffer(false);

    const blob = new Blob([buffer]);

    const url = URL.createObjectURL(blob);

    const link = document.createElement("a");

    link.href = url;
    link.download = "NEXUS_Model.frag";

    link.click();

    URL.revokeObjectURL(url);

    progress.textContent =
      "NEXUS fragment export completed.";

  } catch (error) {
    console.error("NEXUS EXPORT ERROR:", error);

    progress.textContent =
      "Fragment export failed.";
  }
});

// ==========================================
// APPLICATION READY
// ==========================================

console.log("NEXUS Browser Viewer initialized.");
console.log("NEXUS | ARCHITECT ARZATH");
