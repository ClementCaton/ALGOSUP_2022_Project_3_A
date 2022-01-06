# Header composition of a WAV file

## File header
- File type
  - 4 bytes, 1 byte per character
  - Is always "RIFF"
- File size
  - 4 bytes = 32-bit integer
  - Size of the file from this point (file size - 16 bytes)
  - Generally set after after writing the rest of the file
- File subtype
  - 4 bytes, 1 byte per character
  - In our case, always "WAVE"

## Format header
- Format chunk marker
  - 4 bytes, 1 byte per character
  - Is always "fmt "
- Format header size
  - 4 bytes = 32-bit integer
  - Is always 16
- PCM type
  - 2 bytes = 16-bit integer
  - In out case, always 1
- Channel number
  - 2 bytes = 16-bit integer
  - Number of channels to play sound on
- Sample rate
  - 4 bytes = 32-bit integer
  - Frequency at which to sample the sound (= number of data points in a second)
  - Is usually 44100 or 48000
- Byte rate
  - 4 bytes = 32-bit integer
  - Number of bytes used for 1 second of audio
  - Formula: `Sample Rate * Nb of Channels * Nb of Bits per Sample / 8`
- Block align / frame size
  - 2 bytes = 16-bit integer
  - Number of bytes across all channels
  - Formula: `Nb of Channels * Nb of Bits per Sample / 8`
- Bits per sample
  - 2 bytes = 16-bit integer
  - Number of bits to represent a sample

## Data header
- Data chunk marker
  - 4 bytes, 1 byte per character
  - Is always "data"
- Data size
  - 4 bytes = 32-bit integer
  - Number of bytes composing the data
  - Formula: `Nb of Samples * Nb of Channels * Nb of Bits per Sample / 8`