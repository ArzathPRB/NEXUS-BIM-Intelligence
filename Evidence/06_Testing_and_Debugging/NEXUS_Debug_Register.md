# NEXUS - Testing and Debugging Register
Developer: Architect Arzath
Date: 14 November 2026

| ID | Issue | Resolution | Final Status |
|---|---|---|---|
| DBG-01 | Revit document modification without transaction | Transaction handling addressed | Reported resolved |
| DBG-02 | Revit DLL locked during build | Revit process closed before rebuilding | Resolved |
| DBG-03 | Browser dependency installation | Dependencies installed and verified | Resolved |
| DBG-04 | Three.js TypeScript declarations missing | Installed @types/three | Resolved |
| DBG-05 | Unused modelBounds variable | Removed unused declaration and assignment | Resolved |
| DBG-06 | Duplicate camera framing code | Removed obsolete lines 130-187 | Build verified |
| DBG-07 | Empty Three.js bounding box | Changed to fragment-aware BoundingBoxer approach | Runtime verification pending |
| DBG-08 | Vite localhost connection refused | Restarted development server | Resolved |
| DBG-09 | Missing favicon.ico | Identified harmless 404 | Non-critical |

## Build Verification
Browser Viewer production build: Successful.

## Outstanding Tests
- Automatic model framing
- BIM hierarchy
- Element selection
- IFC properties
- Category visibility

## Evidence Rule
Attach original screenshots, logs and source-code references
where available. Do not treat an unverified fix as completed.
