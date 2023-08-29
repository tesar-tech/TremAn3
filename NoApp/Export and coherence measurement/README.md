# Data extractor and measurement of coherence

Current state of the app measures the coherence only for first and second ROI.
This work was done for computing maximum coherence of every combination of ROI's pairs.

These rois are:

- H - Head (first roi, identified by Red color)
- R - Right hand (second roi, Green)
- L - Left hand (third roi, Blue)

And their combinations HR, HL, RL (always in this order).

This was used to compute maximum coherence for every combination of rois and every subject in 2-12 Hz frequency band.

## Data extraction

Treman app saves the data of every measurement in `C:\Users\<username>\AppData\Local\Packages\26872JanTesa.TremAn3_pc8qdhpdg0sz8\LocalState\AllM` in folders named by `v_` prefix, name of the video, random guid.

In this folder there is `.json` with the same name containing the metadata for video. And folder with prefix `m_` containing the measurement result. Every time user clicks "compute" button, new measurement is created.

In `m_` folder there are two `.json` files. One with metadata about the measurement (for example the time range in video and and datetime when the measurement was taken). And second with the data itself (with `_vectorData` suffix).

`_vectorData` contains Com values, same as values for all computations (for example coherence). In this data extractor we care just about the ComX and ComY values.

`ExtractData.ipynb` extract the ComX and ComY values from `_vectorData` and saves them in `data` folder in `csv` format. It goes through colors and understand that the Red roi and its ComX and ComY values are Head. This is the outputed `ComX.csv` frist 2 lines:

```csv
X,H,R,L
0.03,-0.24,-0.15,-0.23
```

There is also X value included, but is actually never used, since we have the FrameRate.

The `ExtractData` creates intended data structure, that goes like:

```bash
- 1_patientName
  - SForw
    - ComX.csv
    - ComY.csv
    - info.json
  - SRest
    - ComX.csv
    - ComY.csv
    - info.json
  - SWing
    - ComX.csv
    - ComY.csv
    - info.json
- 2_patientNameElse
  - ...
```

The patient name is taken from second parent folder from the video path. `SRest`, `SForw` and `SWing` are the names of the videos. The `info.json` contains the metadata about the ROIs (not used further, just for completenss and reproducebility) and FrameRate for measurement (taken from video).

This is the folder structure that data extractor expects the measurement to be taken from:

```bash
- 1_patientName
    - whatever folder name (usually _vid, because videos are stored here. The sibling folder contains other data (e.g. accelerometer data))
      - SForw.mp4
      - SRest.mp4
      - SWing.mp4
      - other videos the script does not care about
- 2_patientNameElse
  - ...
```

This goes for `DataExtractor.ipynb`.

## Coherence measurement

Data are processed by matlab `script.m`. It goes through the strucutre and computes the coherence for every combination of rois and every subject in 2-12 Hz frequency band. Also it ouputs two tables (one with the coherence value, one with the frequency).
This script requires the `data` folder to be in the same folder as the script itself.

Note that this script has hardcoded movement names (SForw, SRest, SWing) and hardcoded rois (H, R, L). That has to match with folder structure and data extractor.

The outputed csv structure is (cutted presition for readability):

```csv
Id,Folder,SForw-HR,SForw-HL,SForw-RL,SRest-HR,SRest-HL,SRest-RL,SWing-HR,SWing-HL,SWing-RL
1,1_patientName,11.3,6.5,8.9,9.9,3.7,5.0,3.5,3.0,2.3
```