Adapted from [idea2ic](http://www.idea2ic.com/File_Formats/MPEG%20Audio%20Frame%20Header.pdf).

# MP3 File

An MP3 file is composed of independent "frames". This means that you can cut the file and add random information in between the frames and it will not affect the playback.

Each frame starts with a 4 bytes (32 bits) header. This header can easily be detected making it easy to differentiate between frames and extra data.

To find a header, just find a byte with all bits set (value 255) followed by a byte with its three most significant bits set (value >= 224). For more information see the [header details](#frame-header) section below.

# Frame header

The header consists of four bytes and is composed of 13 fields composed as follows:

`AAAAAAAA AAABBCCD EEEEFFGH IIJJKLMM`

| Chr | Length | Description                                                                                     |
| --- | ------ | ----------------------------------------------------------------------------------------------- |
| A   | 11     | Frame sync (all bits set)                                                                       |
| B   | 2      | MPEG version: <br> 0. V2.5 <br> 1. Reserved <br> 2. V2 <br> 3. V1                               |
| C   | 2      | Layer description: <br> 0. Reserved <br> 1. Layer III <br> 2. Layer II <br> 3. Layer I          |
| D   | 1      | Protection bit: <br> 0. CRC protected (16 bits after this header) <br> 1. Not protected         |
| E   | 4      | Bitrate index cf table [here](##bitrate-index)                                                  |
| F   | 2      | Sampling rate frequency index cf table [here](##sampling-rate-index)                            |
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
<http://digitalsoundandmusic.com/5-3-8-algorithms-for-audio-companding-and-compression/>
<https://ledgernote.com/blog/q-and-a/how-does-mp3-compression-work/>

Here is an algorithm for mp3 compression
    algorithm MP3 {
        /*Input:  An audio signal in the time domain
        Output:  The same audio signal, compressed
        */
        Process the audio signal in frames
        For each frame {
        Use the Fourier transform to transform the time domain data to the frequency domain, sending
            the results to the psychoacoustical analyzer {
                Based on masking tones and masked frequencies, determine the signal-to-masking noise
                    ratios (SMR) in areas across the frequency spectrum
                Analyze the presence and interactions of transients
            }
        Divide the frame into 32 frequency bands
        For each frequency band {
            Use the modified discrete cosine transform (MDCT) to divide each of the 32 frequency bands
                into 18 subbands, for a total of 576 frequency subbands
            Sort the subbands into 22 groups, called scale factor bands, and based on the SMR, determine
                a scaling factor for each scale factor band
            Use nonuniform quantization combined with scaling factors to quantize
            Encode side information
            Use Huffman encoding on the resulting 576 quantized MDCT coefficients
            Put the encoded data into a properly formatted frame in the bit stream
        }
    }

And now those are the steps more detailled of the algorithm :

### 1. Divide the audio signal in frames

MP3 compression processes the original audio signal in frames of 1152 samples. Each frame is split into two granules of 576 samples each. Frames are encoded in a number of bytes consistent with the bit rate set for the compression at hand. In the example described above (with sampling rate of 44.1 kHz and requested bit rate of 128 kb/s), 1152 samples are compressed into a frame of approximately 450 bytes – 418 bytes for data and 32 bytes for the header.

### 2. Use the Fourier transform to transform the time domain data to the frequency domain, sending the results to the psychoacoustical analyzer

The fast Fourier transform changes the data to the frequency domain. The frequency domain data is then sent to a psychoacoustical analyzer. One purpose of this analysis is to identify masking tones and masked frequencies in a local neighborhood of frequencies over a small window of time. The psychoacoustical analyzer outputs a set of signal-to-mask ratios (SMRs) that can be used later in quantizing the data. The SMR is the ratio between the amplitude of a masking tone and the amplitude of the minimum masked frequency in the chosen vicinity. The compressor uses these values to choose scaling factors and quantization levels such that quantization error mostly falls below the masking threshold. Step 5 explains this process further.

Another purpose of the psychoacoustical analysis is to identify the presence of transients and temporal masking. When the MDCT is applied in a later step, transients have to be treated in smaller window sizes to achieve better time resolution in the encoding. If not, one transient sound can mask another that occurs close to it in time. Thus, in the presence of transients, windows are made one third their normal size in the MDCT.

### 3. Divide each frame into 32 frequency bands

Steps 2 and 3 are independent and actually could be done in parallel. Dividing the frame into frequency bands is done with filter banks. Each filter bank is a bandpass filter that allows only a range of frequencies to pass through. (Chapter 7 gives more details on bandpass filters.) The complete range of frequencies that can appear in the original signal is 0 to ½ the sampling rate, as we know from the Nyquist theorem. For example, if the sampling rate of the signal is 44.1 kHz, then the highest frequency that can be present in the signal is 22.05 kHz. Thus, the filter banks yield 32 frequency bands between 0 and 22.05 kHz, each of width 22050/32, or about 689 Hz.

The 32 resulting bands are still in the time domain. Note that dividing the audio signal into frequency bands increases the amount of data by a factor of 32 at this point. That is, there are 32 sets of 1152 time-domain samples, each holding just the frequencies in its band. (You can understand this better if you imagine that the audio signal is a piece of music that you decompose into 32 frequency bands. After the decomposition, you could play each band separately and hear the musical piece, but only those frequencies in the band. The segments would need to be longer than 1152 samples for you to hear any music, however, since 1152 samples at a sampling rate of 44.1 kHz is only 0.026 seconds of sound.)

### 4. Use the MDCT to divide each of the 32 frequency bands into 18 subbands for a total of 576 frequency subbands

The MDCT, like the Fourier transform, can be used to change audio data from the time domain to the frequency domain. Its distinction is that it is applied on overlapping windows in order to minimize the appearance of spurious frequencies that occur because of discontinuities at window boundaries. (“Spurious frequencies” are frequencies that aren’t really in the audio, but that are yielded from the transform.) The overlap between successive MDCT windows varies depending on the information that the psychoacoustical analyzer provides about the nature of the audio in the frame and band. If there are transients involved, then the window size is shorter for greater temporal resolution. Otherwise, a larger window is used for greater frequency resolution.

### 5. Sort the subbands into 22 groups, called scale factor bands, and based on the SMR, determine a scaling factor for each scale factor band. Use nonuniform quantization combined with scaling factors to quantize

Values are raised to the ¾ power before quantization. This yields nonuniform quantization, aimed at reducing quantization noise for lower amplitude signals, where it has a more harmful impact.

The psychoacoustical analyzer provides information that is the basis for sorting the subbands into scale factor bands. The scale factor bands cover several MDCT coefficients and more closely match the critical bands of the human ear. This is one of the ways in which MP3 is an improvement over MPEG-1 Layers 1 and 2.

All bands are quantized by dividing by the same value. However, the values in the scale factor bands can be scaled up or down based on their SMR. Bands that have a lower SMR are multiplied by larger scaling factors because the quantization error for these bands has less impact, falling below the masking threshold.

Consider this example. Say that an uncompressed band value is 20,000 and values from all bands are quantized by dividing by 128 and rounding down. Thus the quantized value would be 156. When the value is restored by multiplying by 128, it is 19,968, for an error of
3220000=0.0016
. Now supposed the psychoacoustical analyzer reveals that this band requires less precision because of a strong masking tone. Thus, it determines that the band should be scaled by a factor of 0.1. Now we have
floor(20000∗0.1128)=15
. Restoring the original value we get
15∗128=19200
, for an error of
80020000=0.04
.

An appropriate psychoacoustical analysis provides scaling factors that increase the quantization error where it doesn’t matter, in the presence of masking tones.  Scale factor bands effectively allow less precision (i.e., fewer bits) to store values if the resulting quantization error falls below the audible level. This is one way to reduce the amount of data in the compressed signal.

### 6. Encode side information

Side information is the information needed to decode the rest of the data, including where the main data begins, whether granule pairs can share scale factors, where scale factors and Huffman encodings begin, the Huffman table to use, the quantization step, and so forth.

### 7. Use Huffman encoding on the resulting 576 quantized MDCT coefficients

The result of the MDCT is 576 coefficients representing the amplitudes of 576 frequency subbands. Huffman encoding is applied at this point to further compress the signal.

Huffman encoding is a compression method that assigns shorter codes to symbols that appear more often in the signal or file being encoded and longer codes for symbols that appear infrequently. Here’s a sketch of how it works. Imagine that you are encoding 88 notes from a piano keyboard. You can use 7 bits for each note. (Six bits would be too few, since with six bits you can represent only
26=64
different notes.) In a particular music file, you have 100 instances of notes. The number of instances of each is recorded in the table below. Not all 88 notes are used, but this is not important to the example. (This is just a contrived example, so don’t try to make sense of it musically.)

[table width=”30%”]

Note,Instances of Note
C4,31
B4,20
F3,16
D4,11
G4,8
E4,5
A3,4
B3,3
F4,2

[/table]

There are 100 notes to be encoded here. If each requires 7 bits, that’s 700 bits for the encoded file. It’s possible to reduce the size of this encoded file if we use fewer than 7 bits for the notes that appear most often. We can choose a different encoding by building what’s called a Huffman tree. We begin by creating a node (just a circle in a graph) for each of the notes in the file and the number of instances of that note, like this:

Huffman_01

(The initial placement of the nodes isn’t important, although a good placement can make the tree look neater after you’ve finished joining nodes.) Now the idea is to combine the two nodes that have the smallest value, making a node above, marking it with the sum of the chosen nodes’ values, and joining the new node with lines to the nodes below. (The lines connecting the nodes are called arcs.) B3/3 and F4/2 have the smallest values, so we join them, like this:

Huffman_02

At the next step, A3/4 and the 5 node just created can be combined. (We could also combine A3/4 with E4/5. The choice between the two is arbitrary.) The sum of the instances is 9.

Huffman_03

In the next two steps, we combine E4/5 with 9 and then D4 with G4, as shown.

Huffman_04

Continuing in this manner, the final tree becomes this:

Huffman_05

Notice that the left arc from each node has been labeled with a 0 and the right with a 1. The nodes at the bottom represent the notes to be encoded. You can get the code for a note by reading the 0s and 1s on the arcs that take you from the top node to one of the notes at the bottom. For example, to get to B4, you follow the arcs labeled 0 0. Thus, the new encoding for B4 is 0 0, which requires only two bits. To get to B3, you follow the arcs labeled 1 1 1 1 1 0. The new encoding for B3 is 1 1 1 1 1 0, which requires six bits. All the new encodings are given in the table below:

[table width=”50%”]

Note,Instances of Note,Code in Binary,Number of Bits
C4,31,10,2
B4,20,0,2
F3,16,110,3
D4,11,10,3
G4,8,11,3
E4,5,1110,4
A3,4,11110,5
B3,3,111110,6
F4,2,111111,6

[/table]

Note that not all notes are encoded in the same number of bits. This is not a problem to the decoder because no code is a prefix of any other. (This way, the decoder can figure out where a code ends without knowing its length.) With the encoding that we just derived, which uses fewer bits for notes that occur frequently, we need only 31*2 + 20*2 + 16*3 + 11*3 + 8*3 + 5*4 + 4*5 + 3*6 + 2*6 = 277 bits as opposed to the 700 needed previously.

This illustrates the basic concept of Huffman encoding. However, it is realized in a different manner in the context of MP3 compression. Rather than generate a Huffman table based on the data in a frame, MP3 uses a number of predefined Huffman tables defined in the MPEG standard. A variable in the header of each frame indicates which of these tables is to be used.

Steps 5, 6, and 7 can be done iteratively. After an iteration, the compressor checks that the noise level is acceptable and that the proper bit rate has been maintained. If there are more bits that could be used, the quantizer can be reduced. If the bit limit has been exceeded, quantization must be done more coarsely. The level of distortion in each band is also analyzed. If the distortion is not below the masking threshold, then the scale factor for the band can be adjusted.

### 8. Put the encoded data into a properly formatted frame in the bit stream

The header of each frame is as shown in Table 5.4. The data part of each frame consists of the scaled, quantized MDCT values.

AAC compression, the successor to MP3, uses similar encoding techniques but improves on MP3 by offering more sampling rates (8 to 96 kHz), more channels (up to 48), and arbitrary bit rates. Filtering is done solely with the MDCT, with improved frequency resolution for signals without transients and improved time resolution for signals with transients. Frequencies over 16 kHz are better preserved. The overall result is that many listeners find AAC files to have better sound quality than MP3 for files compressed at the same bit rate.
