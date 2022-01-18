Adapted from (idea2ic)[http://www.idea2ic.com/File_Formats/MPEG%20Audio%20Frame%20Header.pdf].


# MP3 File

An MP3 file is composed of independent "frames". This means that you can cut the file and add random information in between the frames and it will not affect the playback.

Each frame starts with a 4 bytes (32 bits) header. This header can easily be detected making it easy to differentiate between frames and extra data.

To find a header, just find a byte with all bits set (value 255) followed by a byte with its three most significant bits set (value >= 224). For more information see the (header details)[#frame-header] section below.



# Frame header

The header consists of four bytes and is composed of 13 fields composed as follows:

`AAAAAAAA AAABBCCD EEEEFFGH IIJJKLMM`

| Chr | Length | Description                                                                                     |
| --- | ------ | ----------------------------------------------------------------------------------------------- |
| A   | 11     | Frame sync (all bits set)                                                                       |
| B   | 2      | MPEG version: <br> 0. V2.5 <br> 1. Reserved <br> 2. V2 <br> 3. V1                               |
| C   | 2      | Layer description: <br> 0. Reserved <br> 1. Layer III <br> 2. Layer II <br> 3. Layer I          |
| D   | 1      | Protection bit: <br> 0. CRC protected (16 bits after this header) <br> 1. Not protected         |
| E   | 4      | Bitrate index cf table (here)[##bitrate-index]                                                  |
| F   | 2      | Sampling rate frequency index cf table (here)[##sampling-rate-index]                            |
| G   | 1      | Padding: <br> 0. Frame not padded <br> 1. Padded with one extra bit                             |
| H   | 1      | Private bit                                                                                     |
| I   | 2      | Channel mode: <br> 0. Stereo <br> 1. Joint stereo <br> 2. Dual stereo <br> 3. Mono              |
| J   | 2      | Mode extension (Joint stereo only): <br> First bit: MS stereo <br> Second bit: Intensity stereo |
| K   | 1      | Copyright                                                                                       |
| L   | 1      | Original media                                                                                  |
| M   | 2      | Emphasis: <br> 0. None <br> 1. 50/15 ms <br> 2. Reserved <br> 3. CCIT J.17                      |



## Bitrate index

Values are in kilobits per seconds.

Indexes are the MPEG Version and the Layer version

Example: `2,3` = MPEG Version 2 or 2.5 and Layer III

| Bits | 1,1 | 1,2 | 1,3 | 2,1 | 2,2 | 2,3 |
| ---- | --- | --- | --- | --- | --- | --- |
| 0001 | 32  | 32  | 32  | 32  | 32  | 8   |
| 0010 | 64  | 48  | 40  | 64  | 48  | 16  |
| 0011 | 96  | 56  | 48  | 96  | 56  | 24  |
| 0100 | 128 | 64  | 56  | 128 | 64  | 32  |
| 0101 | 160 | 80  | 64  | 160 | 80  | 64  |
| 0110 | 192 | 96  | 80  | 192 | 96  | 80  |
| 0111 | 224 | 112 | 96  | 224 | 112 | 56  |
| 1000 | 256 | 128 | 112 | 256 | 128 | 64  |
| 1001 | 288 | 160 | 128 | 288 | 160 | 128 |
| 1010 | 320 | 192 | 160 | 320 | 192 | 160 |
| 1011 | 352 | 224 | 192 | 352 | 224 | 112 |
| 1100 | 384 | 256 | 224 | 384 | 256 | 128 |
| 1101 | 416 | 320 | 256 | 416 | 320 | 256 |
| 1110 | 448 | 384 | 320 | 448 | 384 | 320 |

Special values: <br>
`0000 (0)`: Variable bitrate <br>
`1111 (7)`: Value not allowed



## Sampling rate index

Values in hertz

| Bits | MPEG1    | MPEG2    | MPEG2.5  |
| ---- | -------- | -------- | -------- |
| 00   | 44100    | 22050    | 11025    |
| 01   | 48000    | 24000    | 12000    |
| 10   | 32000    | 16000    | 8000     |
| 11   | Reserved | Reserved | Reserved |



## Frame size

The frame size can be calculated with this formula:

`FrameSize = 144 * BitRate / SampleRate + Padding`



# Audio tags

## MPEG Audio Tag MP3v1

It contains description for the file such as the artist, album, genre, etc...
It can be found at the end of the file (last 128 bytes).

```AAABBBBB BBBBBBBB BBBBBBBB BBBBBBBB
BCCCCCCC CCCCCCCC CCCCCCCC CCCCCCCD
DDDDDDDD DDDDDDDD DDDDDDDD DDDDDEEE
EFFFFFFF FFFFFFFF FFFFFFFF FFFFFFFG
```

| Chr | Length | Description |
| --- | ------ | ----------- |
| A   | 3      | Value "TAG" |
| B   | 30     | Title       |
| C   | 30     | Artist      |
| D   | 30     | Album       |
| E   | 4      | Year        |
| F   | 30     | Comment     |
| G   | 1      | Genre id    |

# Compression
Those two links explains how the compression of the audio is made for mp3 files. The first one is really complete and explains a lot of things about mp3 files.
http://digitalsoundandmusic.com/5-3-8-algorithms-for-audio-companding-and-compression/ 
https://ledgernote.com/blog/q-and-a/how-does-mp3-compression-work/