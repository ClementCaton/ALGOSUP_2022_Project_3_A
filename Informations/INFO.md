# Header composition of a WAV file

## File header

| Name         | Size    | Value                | Description                                            |
| ------------ | ------- | -------------------- | ------------------------------------------------------ |
| File type    | 4 bytes | Constant "RIFF"      |                                                        |
| File size    | 4 bytes | file size - 16 bytes | Generally set after after writing the rest of the file |
| File subtype | 4 bytes | Constant "WAVE"      |                                                        |

## Format header

| Name                | Size    | Value                                         | Description                                      |
| ------------------- | ------- | --------------------------------------------- | ------------------------------------------------ |
| Format chunk marker | 4 bytes | Constant "fmt "                               |                                                  |
| Format chunk size   | 4 bytes | Constant 16                                   |                                                  |
| PCM type            | 2 bytes | Variable (1 in out case)                      |                                                  |
| Channel number      | 2 bytes | Variable                                      | Number of channels to play sound on              |
| Sample rate         | 4 bytes | Usually 8000, 44100 or 48000                  | Frequency at which to sample the sound           |
| Byte rate           | 4 bytes | `SampleRate * NbChannels * BitsPerSample / 8` | Number of bytes used for 1 second of audio       |
| Frame size          | 2 bytes | `NbChannels * BitsPerSample / 8`              | Number of bytes across all channels for 1 sample |
| Bits per sample     | 2 bytes | Variable                                      | Number of bits to represent a sample             |

## Data header

| Name              | Size    | Value                                        | Description                        |
| ----------------- | ------- | -------------------------------------------- | ---------------------------------- |
| Data chunk marker | 4 bytes | Constant "data"                              |
| Data size         | 4 bytes | `NbSamples * NbChannels * BitsPerSample / 8` | Number of bytes composing the data |
