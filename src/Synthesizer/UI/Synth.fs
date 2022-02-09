namespace Synthesizer

open System
open System.IO

[<StructuredFormatDisplay("{AsString}")>]
type Synth(?baseBpm:float, ?baseSampleRate:float, ?baseWaveType:BaseWaves, ?basePlatform:bool) =

    member val bpm = defaultArg baseBpm 90.
        with get, set
    
    member x.SetBpm newBpm =
        x.bpm <- newBpm
        List.empty<float>
    
    member val sampleRate = defaultArg baseSampleRate 44100.
        with get, set

    member val waveType = defaultArg baseWaveType Sin
        with get, set

    override x.ToString() = $"sampleRate: {x.sampleRate} \nbpm: {x.bpm} \ndefault wave type: {x.waveType}"
    member x.AsString = x.ToString()

    /// <summary>
    /// Calculates a frequency with the base A4=440 Hz
    /// </summary>
    /// <param name="octave">Octave of the note</param>
    /// <param name="note">Value of the note</param>
    /// <returns>Frequency of the note</returns>
    
    member x.GetNoteFreq (note:Note) (octave:int) =
        CalcNoteFreq(note, octave).Output



    /// <summary>
    /// Calculates a frequency given the A4 frequency
    /// </summary>
    /// <param name="octave">Octave of the note</param>
    /// <param name="note">Value of the note</param>
    /// <param name="aFourFreq">Frequency of A4</param>
    /// <returns>Frequency of the note</returns>
    
    member x.GetNoteFreqOffset (note:Note) (octave:int) (aFourFreq:float) =
    
        CalcNoteFreq(note, octave, aFourFreq).Output



    /// <summary>
    /// Creates a sound with a single note
    /// It is recommended to use the method Note instead
    /// </summary>
    /// <param name="frequency">Frequency of the note</param>
    /// <param name="duration">Duration of the sound</param>
    /// <param name="waveType">Sound generator</param>
    /// <returns>Sound data</returns>
    
    member x.Sound (frequency:float) (duration:Duration) (waveType:BaseWaves) =
        let data = SoundData(frequency0 = frequency, duration0 = duration, bpm0 = x.bpm) // TEMP: Remove bpm
        //! The "1." was supposed to be "(data.overDrive)"
        Utility.Overdrive 1. (data.Create(waveType))



    /// <summary>
    /// Creates a sound with a single note in an envelope
    /// </summary>
    /// <param name="frequency">Frequency of the note</param>
    /// <param name="duration">Duration of the sound</param>
    /// <param name="waveType">Sound generator</param>
    /// <param name="sustain">Sustain duration</param>
    /// <param name="attack">Attack duration</param>
    /// <param name="hold">Hold duration</param>
    /// <param name="decay">Decay amplitude</param>
    /// <param name="release">Release duration</param>
    /// <returns>Enveloped sound</returns>
    
    member x.SoundWithEnveloppe (frequency:float) (duration:Duration) (waveType:BaseWaves) (sustain:float) (attack:float) (hold:float) (decay:float) (release:float) = // time, time, time, amp, time
        let data = SoundData(frequency0 = frequency, duration0 = duration, bpm0 = x.bpm) // TEMP: Remove bpm
        //! The "1." was supposed to be "(data.overDrive)"
        Utility.Overdrive 1. (data.CreateWithEnvelope waveType sustain attack hold decay release)



    /// <summary>
    /// Creates a sound with a single note in a custom envelope
    /// </summary>
    /// <param name="frequency">Frequency of the note</param>
    /// <param name="duration">Duration of the sound</param>
    /// <param name="waveType">Sound generator</param>
    /// <param name="dataPoints">List of datapoints on which the sound is pushed towrds (time, amp)</param>
    /// <returns>Enveloped sound</returns>
    
    member x.SoundWithCustomEnveloppe (frequency:float) (duration:Duration) (waveType:BaseWaves) (dataPoints: List<float * float>) = // (time, amp)
        let data = SoundData(frequency0 = frequency, duration0 = duration, bpm0 = x.bpm)
        //! The "1." was supposed to be "(data.overDrive)"
        Utility.Overdrive 1. (data.CreateFromDataPoints waveType dataPoints)



    /// <summary>
    /// Writes a music to a .wav file.
    /// </summary>
    /// <param name="filename">Name of the output file</param>
    /// <param name="music">Music to be saved</param>
    
    member x.WriteToWav (filename:string) (music:List<List<float>>) =
        Directory.CreateDirectory("./Output/") |> ignore
        use stream = File.Create("./Output/" + filename)
        WriteWav().Write (stream) (music)



    /// <summary>
    /// Writes a music to a .wav file
    /// </summary>
    /// <param name="path">Path of the output folder</param>
    /// <param name="filename">Name of the output file</param>
    /// <param name="music">Music to be saved</param>
    
    member x.WriteToWavWithPath (path:string) (filename:string) (music:List<List<float>>) =
        Directory.CreateDirectory(path) |> ignore
        use stream = File.Create(path + filename)
        WriteWav().Write (stream) (music)



    /// <summary>
    /// Get information on an MP3 file
    /// </summary>
    /// <param name="name">Name of the MP3 file</param>
    /// <returns>Data stored in the MP3 header</returns>

    member x.ReadFromMP3Header (name: string) =
        if name.Contains(".mp3") then
            readMP3(File.Open("./Output/" + name, FileMode.Open)).mp3Decoding
        else
            readMP3(File.Open("./Output/" + name + ".mp3", FileMode.Open)).mp3Decoding



    /// <summary>
    /// Get information on an MP3 file with a specific path
    /// </summary>
    /// <param name="path">Path to the MP3 file</param>
    /// <returns>Data stored in the MP3 header</returns>

    member x.ReadFromMP3HeaderWithPath (path: string) =
        if path.Contains(".mp3") then
            readMP3(File.Open(path, FileMode.Open)).mp3Decoding
        else
            readMP3(File.Open(path+".mp3", FileMode.Open)).mp3Decoding



    /// <summary>
    /// Reads a music from a .wav file
    /// </summary>
    /// <param name="filename">Name of the input file</param>
    /// <returns>Music from the file</returns>
    
    member x.ReadFromWav (name: string) =
        if name.Contains(".wav") then
            ReadWav().Read (File.Open("./Output/"+name, FileMode.Open))
        else
            ReadWav().Read (File.Open("./Output/"+name+".wav", FileMode.Open))



    /// <summary>
    /// Reads a music from a .wav file
    /// </summary>
    /// <param name="filepath">Path of the input file</param>
    /// <returns>Music from the file</returns>
    
    member x.ReadFromWavWithPath (path: string) =
        if path.Contains(".wav") then
            ReadWav().Read (File.Open(path, FileMode.Open))
        else
            ReadWav().Read (File.Open(path+".wav", FileMode.Open))



    /// <summary>
    /// Creates a sound with a single note
    /// </summary>
    /// <param name="duration">Duration of the note</param>
    /// <param name="mNote">Values of the note</param>
    /// <param name="octave">Octave of the note</param>
    /// <returns>Sound represented by a list of samples</returns>
    
    member x.Note (duration:Duration) (mNote:Note) (octave:int) =
        let freq = x.GetNoteFreq mNote octave
        x.Sound freq duration x.waveType



    /// <summary>
    /// Creates a sound with silence
    /// </summary>
    /// <param name="duration">Duration of the silence/rest</param>
    /// <returns>Sound with silence</returns>
    
    member x.Silence (duration:Duration) =
        x.Sound 0 duration Silence



    /// <summary>
    /// Creates a chord with different notes 
    /// </summary>
    /// <param name="duration">Duration of the chord</param>
    /// <param name="rootNote">Base note value for the chord</param>
    /// <param name="chordQuality">Type of chord</param>
    /// <param name="rootOctave">Octave of the base note</param>
    /// <returns>Sound with the cord</returns>
        
    member x.Chord (duration:Duration) (rootNote:Note) (chordQuality:ChordQuality) (rootOctave:int) =
        let calc = CalcNoteFreq(rootNote, rootOctave)
        SoundData().GetChord chordQuality
        |> List.map (fun offset -> calc.Offset offset)
        |> List.map (fun freq -> x.Sound freq duration x.waveType)
        |> Utility.AddMean
    


    /// <summary>
    /// Concatenates sounds with parameterizable fade in/out
    /// </summary>
    /// <param name="corner">Number of data points to apply the fading</param>
    /// <param name="sounds">List of sounds to concatenate</param>
    /// <returns>Concatenated sound</returns>
    
    member x.ComposeCutCorner (corner:int) (sounds:List<List<float>>) =
        sounds |> List.map(fun x -> Utility.CutCorners corner x) |> List.concat
    


    /// <summary>
    /// Concatenates sounds with a default fade of 100 points
    /// </summary>
    /// <param name="sounds">List of sounds to concatenate</param>
    /// <returns>Concatenated sound</returns>
    
    member x.Compose (sounds:List<List<float>>) = x.ComposeCutCorner 100 sounds
    


    /// <summary>
    /// Concatenates sounds without fading
    /// </summary>
    /// <param name="sounds">List of sounds to concatenate</param>
    /// <returns>Concatenated sound</returns>
    
    member x.ComposeNoCutCorner (sounds:list<list<float>>) = List.concat sounds
    


    /// <summary>
    /// Superpose different voices or notes of a chord
    /// </summary>
    /// <param name="sounds">List of sounds to superpose</param>
    /// <returns>Superpose sound</returns>
    
    member x.Add sounds = Utility.AddMean sounds



    /// <summary>
    /// Creates a chart plotting the given data list
    /// </summary>
    /// <param name="title">Title of the plot</param>
    /// <param name="data">List of float</param>
    
    member x.Preview (title:string) (data:List<float>) =
        Preview.Chart title data
        data



    /// <summary>
    /// Creates a chart plotting the given data map
    /// </summary>
    /// <param name="title">Title of the plot</param>
    /// <param name="map">Map of float to float</param>
    
    member x.PreviewMap (title:string) (map:Map<float, float>) =
        map
        |> Map.toList
        |> List.unzip
        ||> Preview.ChartXY title
        map



    /// <summary>
    /// Apply a function to each the channels of a music
    /// </summary>
    /// <param name="func">Function to apply</param>
    /// <param name="channels">Different channels of sound</param>
    /// <returns>The new music with the function applied</returns>
    
    member x.ForAllChannels func (channels:List<List<float>>) =
        channels |> List.map func



    /// <summary>
    /// Applies Fourier analysis on sound data
    /// </summary>
    /// <param name="data">Data to analyse</param>
    /// <returns>Map of frequency indices to amplitude</returns>
    
    member x.Fourier (data:List<float>) =
        FrequencyAnalysis.Fourier x.sampleRate data



    /// <summary>
    /// Cuts off the start of the inputed sound data
    /// </summary>
    /// <param name="time">The amount of time to be cut off in seconds</param>
    /// <param name="data">Data to be edited</param>
    /// <returns>The end of the inputed sound data</returns>
    
    member x.CutStart (time:float) (data:List<float>) =
        Utility.CutStart x.sampleRate time data



    /// <summary>
    /// Cuts off the end of the inputed sound data
    /// </summary>
    /// <param name="time">The amount of time to be cut off in seconds</param>
    /// <param name="data">Data to be edited</param>
    /// <returns>The start of the inputed sound data</returns>
    
    member x.CutEnd (time:float) (data:List<float>) =
        Utility.CutEnd x.sampleRate time data



    /// <summary>
    /// Cuts out the middle of the inputed sound data
    /// </summary>
    /// <param name="timeStart">The amount of time to be kept at the start in seconds</param>
    /// <param name="timeEnd">The amount of time to be kept at the end in seconds</param>
    /// <param name="data">Data to be edited</param>
    /// <returns>The edges of the inputed sound data</returns>
    
    member x.CutMiddle (timeStart:float) (timeEnd:float) (data:List<float>) =
        Utility.CutEnd x.sampleRate (float data.Length/x.sampleRate - timeStart) data @ Utility.CutStart x.sampleRate (float data.Length/x.sampleRate - timeEnd) data



    /// <summary>
    /// Cuts off the edges of the inputed sound data
    /// </summary>
    /// <param name="Cuts off the end of the inputed sound data"></param>
    /// <param name="The amount of time to be cut off from the start in seconds"></param>
    /// <param name="The amount of time to be cut off from the end in seconds"></param>
    /// <returns>The edges of the inputed sound data</returns>
    
    member x.CutEdge timeStart timeEnd (data:List<float>) =
        Utility.CutStart x.sampleRate timeStart (Utility.CutEnd x.sampleRate timeEnd data)
        


    /// <summary>
    /// Applies fade in and out to smooth both ends
    /// </summary>
    /// <param name="limit">Number of data points to fade</param>
    /// <param name="data">Data to fade</param>
    /// <returns>Smoothed data</returns>
    
    member x.CutCorners (limit:int) (data:List<float>) =
        Utility.CutCorners limit data



    /// <summary>
    /// Applies multiple filters simultaneously
    /// </summary>
    /// <param name="filters">List of filters to apply</param>
    /// <param name="data">Sound data to filter</param>
    /// <returns>Filtered data</returns>
    
    member x.ApplyFilters filters data =
        Filter.ApplyFilters filters data



    /// <summary>
    /// Plays a .wav file from raw data with an offset
    /// </summary>
    /// <param name="offset">When to start playing in seconds</param>
    /// <param name="data">Data of the sound to play</param>

    member x.PlayWav (offset:float) data =
        match int Environment.OSVersion.Platform with
        | 4| 6 -> 
            x.WriteToWavWithPath "./Output/temp_file_storage/" ".tempFile.wav" data
            PlayMusic.PlayMac offset "./Output/temp_file_storage/.tempFile.wav" 
            File.Delete "./Output/temp_file_storage/.tempFile.wav"
            Directory.Delete "./Output/temp_file_storage/"
        | _       ->  
            let stream = new MemoryStream()
            WriteWav().Write stream data 
            PlayMusic.PlayWithOffset offset stream 



    /// <summary>
    /// Plays a .wav file from a path with an offset
    /// </summary>
    /// <param name="offset">When to start playing in seconds</param>
    /// <param name="filePath">Path of the file</param>

    member x.PlayWavFromPath (offset:float) (filePath:string) =
        match int Environment.OSVersion.Platform with
        | 4| 6 -> 
            PlayMusic.PlayMac offset ("./Output/" + filePath)
        | _ ->  
            PlayMusic.PlayWithOffsetFromPath offset ("./Output/" + filePath)

    //Filters

    /// <summary>
    /// Changes the amplitude of the inputted audio data
    /// </summary>
    /// <param name="multiplier">Ratio at which the amplitude needs to be changed at</param>
    /// <param name="data">Audio data</param>
    /// <returns>Audio data with the new amplitude</returns>
    
    member x.ChangeAmplitude (multiplicator:float) (data:List<float>) =
        Filter.ChangeAmplitude multiplicator data

    /// <summary>
    /// Additions two waves with ratio between them
    /// </summary>
    /// <param name="ratio">The ratio between the two waves</param>
    /// <param name="dataX">First audio input</param>
    /// <param name="dataY">Second audio input</param>
    /// <returns>The sum of the two inputted audios</returns>

    member x.AddTwoWaves (ratio:float) (dataX:List<float>) (dataY:List<float>) =
        Filter.AddTwoWaves ratio dataX dataY
    
    /// <summary>
    /// Repeats the inputted audio data with a preset delay and changes the amplitude every new repetition
    /// </summary>
    /// <param name="nbEcho">Number of times the audio should be repeated</param>
    /// <param name="decay">How much the amplitude of the echos should be changed (multiplied)</param>
    /// <param name="delay">The delay for each new repetition (starts at 0, NOT at the end of the audio)</param>
    /// <param name="sampleRate">The inputs sample rate</param>
    /// <param name="dryData">The original audio</param>
    /// <returns>Repeated audio</returns>

    member x.Repeater (nbEcho:int) (decay:float) (delay:float) (dryData:List<float>) =
        Filter.Repeater nbEcho decay delay x.sampleRate dryData

    /// <summary>
    /// Creates a reverberating effect
    /// </summary>
    /// <param name="delayRatio">At which point the sound should be repeated (1s sound with 0.6 delay = 0.6s delay)</param>
    /// <param name="minAmpRatio">Amplitude at which the sound should not be repeated anymore</param>
    /// <param name="decay">How much the amplitude of the echos should be changed (multiplied)</param>
    /// <param name="sampleRate">The inputs sample rate</param>
    /// <param name="dryData">The original audio</param>
    /// <returns>Filtered audio</returns>

    member x.Reverb (delayRatio:float) (minAmpRatio:float) (decay:float) (dryData:List<float>) =
        Filter.Reverb delayRatio minAmpRatio decay x.sampleRate dryData

    /// <summary>
    /// Creates an echo sound effect
    /// </summary>
    /// <param name="nbEcho">Number of times the audio should be repeated</param>
    /// <param name="decay">How much the amplitude of the echos should be changed (multiplied)</param>
    /// <param name="delay">The delay for each new repetition (starts at the end of the audio)</param>
    /// <param name="sampleRate">The inputs sample rate</param>
    /// <param name="dryData">The original audio</param>
    /// <returns>Filtered audio</returns>
    
    member x.Echo (nbEcho:int) (decay:float) (delay:float) (dryData:List<float>) =
        Filter.Echo nbEcho decay delay x.sampleRate dryData

    /// <summary>
    /// Adds a sweeping sound effect to the audio
    /// </summary>
    /// <param name="Delay">Delay at which the effect should start at (in ms)</param>
    /// <param name="speed">How fast should the effect get stronger</param>
    /// <param name="sampleRate">The inputs sample rate</param>
    /// <param name="bpm">Beats per minute</param>
    /// <param name="dryData">The original audio</param>
    /// <returns>Filtered audio</returns>

    member x.Flanger (delay:float) (speed:float) (dryData:List<float>) =
        Filter.Flanger delay speed x.sampleRate x.bpm dryData

    /// <summary>
    /// Creates a custom pattern for the inputted audios amplitude to follow
    /// </summary>
    /// <param name="dataPoints0">Coordinates for the envelope to follow [(time1 en s, amp1); (time2 en s, amp2)]</param>
    /// <param name="sampleRate">The inputs sample rate</param>
    /// <param name="data">Audio data</param>
    /// <returns>Edited sound</returns>

    member x.CustomEnvelope (dataPoints: List<float * float>) (data:List<float>) =
        Filter.CustomEnvelope dataPoints x.sampleRate data

    /// <summary>
    /// The envelope is the way a sound change over time.
    /// </summary>
    /// <param name="sustain">The level of output while a sustain instruction persists (held note).</param>
    /// <param name="attack">The amount of time it takes for the envelop to reach the end of that first stage, usually the peak level.</param>
    /// <param name="hold0">Adjust the time that the peak amplitude level is held before the decay stage of the envelope begins</param>
    /// <param name="decay0">The amount of time it takes for the envelope to decrease to some specified sustain level</param>
    /// <param name="release0">The time it takes for the output to decrease to zero after the key is released or the sustain instruction ends.</param>
    /// <param name="sampleRate">The inputs sample rate</param>
    /// <param name="data">Audio data</param>
    /// <returns>Edited Sound</returns>
    
    member x.Envelope (sustain:float) (attack:float) (hold:float) (decay:float) (release:float) (data:List<float>) =
        Filter.Envelope sustain attack hold decay release x.sampleRate data 
    
    /// <summary>
    /// Amplitude modulation using a low frequency oscillator.
    /// </summary>
    /// <param name="frequency">The frequency of the modulation </param>
    /// <param name="minAmplitude">The min amplitude of the sound</param>
    /// <param name="maxAmplitude">The max amplitude of the sound</param>
    /// <param name="sampleRate">The inputs sample rate</param>
    /// <param name="data">Audio data</param>
    /// <returns>Edited Sound</returns>

    member x.LFO_AM (frequency:float) (minAmplitude:float) (maxAmplitude:float) (data:List<float>) =
        Filter.LFO_AM frequency minAmplitude maxAmplitude x.sampleRate data
    
    /// <summary>
    /// Frequency modulation using a low frequency oscillator.
    /// </summary>
    /// <param name="modWave">The frequency of the modulation</param>
    /// <param name="multiplier">The min amplitude of the sound</param>
    /// <param name="data">Audio data</param>
    /// <returns>Edited Sound</returns>

    member x.LFO_FM (modWave:List<float>) (multiplicator:float) (data:List<float>) =
        Filter.LFO_FM modWave multiplicator data
    
    /// <summary>
    /// Cuts the sound depending on the frequency
    /// </summary>
    /// <param name="sampleRate">The inputs sample rate</param>
    /// <param name="cutoffFreq">The max frequency at which the sound will be cut</param>
    /// <param name="data">Audio data</param>
    /// <returns>Edited Sound</returns>

    member x.LowPass (cutoffFreq:float) (data:List<float>) =
        Filter.LowPass x.sampleRate cutoffFreq data
    
    /// <summary>
    /// Cuts the sound depending on the frequency
    /// </summary>
    /// <param name="sampleRate">The inputs sample rate</param>
    /// <param name="cutoffFreq">The min frequency at which the sound will be cut</param>
    /// <param name="data">Audio data</param>
    /// <returns>Edited Sound</returns>

    member x.HighPass (cutoffFreq:float) (data:List<float>) =
        Filter.HighPass x.sampleRate cutoffFreq data
    
    /// <summary>
    /// Cuts the sound depending on the frequency
    /// </summary>
    /// <param name="sampleRate">The inputs sample rate</param>
    /// <param name="lowFreq">The min frequency of the interval to be kept</param>
    /// <param name="highFreq">The max frequency of the interval to be kept</param>
    /// <param name="data">Audio data</param>
    /// <returns>Edited Sound</returns>

    member x.BandPass (lowFreq:float) (highFreq:float) (data:List<float>) = 
        Filter.BandPass x.sampleRate lowFreq highFreq data
    
    /// <summary>
    /// Cuts the sound depending on the frequency
    /// </summary>
    /// <param name="sampleRate">The inputs sample rate</param>
    /// <param name="lowFreq">The min frequency of the interval to be erased</param>
    /// <param name="highFreq">The max frequency of the interval to be erased</param>
    /// <param name="data">Audio data</param>
    /// <returns>Edited Sound</returns>

    member x.RejectBand (lowFreq:float) (highFreq:float) (data:List<float>) = 
        Filter.RejectBand x.sampleRate lowFreq highFreq data

    