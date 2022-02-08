namespace Synthesizer

open System
open System.IO

type Synth(?baseBpm:float, ?baseSampleRate:float, ?baseWaveType:BaseWaves, ?basePlatform:bool) =

    member val bpm = defaultArg baseBpm 90.
        with get, set
    
    member val sampleRate = defaultArg baseSampleRate 44100.
        with get, set

    member val waveType = defaultArg baseWaveType Sin
        with get, set

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

    member x.GetNoteFreqOffset (octave:int) (note:Note) (aFourFreq:float) =
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

    member x.ReadFromMP3Header (name: string) =
        if name.Contains(".mp3") then
            readMP3(File.Open("./Output/" + name, FileMode.Open)).mp3Decoding
        else
            readMP3(File.Open("./Output/" + name + ".mp3", FileMode.Open)).mp3Decoding

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
        let freq = x.GetNoteFreq octave mNote
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
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <returns></returns>
    
    member x.CutStart time (data:List<float>) =
        Utility.CutStart x.sampleRate time data



    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <returns></returns>
    
    member x.CutEnd time (data:List<float>) =
        Utility.CutEnd x.sampleRate time data



    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <returns></returns>
    
    member x.CutMiddle timeStart timeEnd (data:List<float>) =
        Utility.CutEnd x.sampleRate (float data.Length/x.sampleRate - timeStart) data @ Utility.CutStart x.sampleRate (float data.Length/x.sampleRate - timeEnd) data



    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <returns></returns>
    
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

    member x.PlayWav offset data =
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

    member x.PlayWavFromPath offset (filePath:string) =
        match int Environment.OSVersion.Platform with
        | 4| 6 -> 
            PlayMusic.PlayMac offset ("./Output/" + filePath)
        | _ ->  
            PlayMusic.PlayWithOffsetFromPath offset ("./Output/" + filePath)
