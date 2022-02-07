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
    
    member val platform = defaultArg basePlatform true
        with get, set

    member x.GetNoteFreq octav note =
        CalcNoteFreq(octav, note).Output

    member x.GetNoteFreqOffset octav note aFourFreq =
        CalcNoteFreq(octav, note, aFourFreq).Output

    member x.Sound freq duration waveType =
        let data = SoundData(frequency0 = freq, duration0 = duration, bpm0 = x.bpm) // TEMP: Remove bpm
        //! The "1." was supposed to be "(data.overDrive)"
        Utility.Overdrive 1. (data.Create(waveType))

    member x.SoundWithEnveloppe freq duration waveType sustain attack hold decay release = // time, time, time, amp, time
        let data = SoundData(frequency0 = freq, duration0 = duration, bpm0 = x.bpm) // TEMP: Remove bpm
        //! The "1." was supposed to be "(data.overDrive)"
        Utility.Overdrive 1. (data.CreateWithEnvelope waveType sustain attack hold decay release)

    member x.WriteToWav name music =
        Directory.CreateDirectory("./Output/") |> ignore
        use stream = File.Create("./Output/" + name)
        WriteWav().Write (stream) (music)

    member x.WriteToWavWithPath path fileName music =
        Directory.CreateDirectory(path) |> ignore
        use stream = File.Create(path + fileName)
        WriteWav().Write (stream) (music)

    member x.ReadFromWav (name: string) =
        if name.Contains(".wav") then
            ReadWav().Read (File.Open("./Output/"+name, FileMode.Open))
        else
            ReadWav().Read (File.Open("./Output/"+name+".wav", FileMode.Open))

    member x.ReadFromWavWithPath (path: string) =
        if path.Contains(".wav") then
            ReadWav().Read (File.Open(path, FileMode.Open))
        else
            ReadWav().Read (File.Open(path+".wav", FileMode.Open))

    member x.ReadFromMP3 (name: string) =
        if name.Contains(".mp3") then
            readMP3(File.Open("./Output/" + name, FileMode.Open)).mp3Decoding
        else
            readMP3(File.Open("./Output/" + name + ".mp3", FileMode.Open)).mp3Decoding

    member x.ReadFromMP3WithPath (path: string) =
        if path.Contains(".mp3") then
            readMP3(File.Open(path, FileMode.Open)).mp3Decoding
        else
            readMP3(File.Open(path+".mp3", FileMode.Open)).mp3Decoding

    member x.Note duration mNote octave =
        let freq = x.GetNoteFreq mNote octave
        x.Sound freq duration x.waveType

    member x.Silence duration =
        x.Sound 0 duration Silence
        
    member x.ComposeCutCorner (corner:int) sounds =
        sounds |> List.map(fun x -> Utility.CutCorners corner x) |> List.concat
            
    member x.Compose = x.ComposeCutCorner 100
    
    member x.ComposeNoCutCorner sounds = List.concat
    
    member x.Add sounds = Utility.Add sounds

    member x.Preview title sound =
        Preview.Chart title sound
        sound

    member x.PreviewMap title map =
        map
        |> Map.toList
        |> List.unzip
        ||> Preview.ChartXY title
        map

    member x.ForAllChannels func channels =
        channels |> List.map func

    member x.Fourier wave =
        FrequencyAnalysis.Fourier(wave)

    member x.Cutstart time (data:List<float>) =
        Utility.CutStart x.sampleRate time data

    member x.CutEnd time (data:List<float>) =
        Utility.CutEnd x.sampleRate time data

    member x.CutMiddle timeStart timeEnd (data:List<float>) =
        Utility.CutEnd x.sampleRate (float data.Length/x.sampleRate - timeStart) data @ Utility.CutStart x.sampleRate (float data.Length/x.sampleRate - timeEnd) data

    member x.CutEdge timeStart timeEnd (data:List<float>) =
        Utility.CutStart x.sampleRate timeStart (Utility.CutEnd x.sampleRate timeEnd data)
        
    member x.CutCorners limit (data:List<float>) =
        Utility.CutCorners limit data

    member x.ApplyFilters filters data =
        Filter.ApplyFilters filters data

    member x.PlayWav (offset:float32) data = 
        if x.platform then
            let stream = new MemoryStream()
            WriteWav().Write stream data 
            PlayMusic.PlayWithOffset offset stream |> ignore
        else
            x.WriteToWavWithPath "./Output/temp_file_storage/" ".tempFile.wav" data
            PlayMusic.PlayMac "./Output/temp_file_storage/.tempFile.wav" offset |> ignore
            File.Delete "./Output/temp_file_storage/.tempFile.wav"
            Directory.Delete "./Output/temp_file_storage/" |> ignore

    member x.PlayWavFromPath offset ((filePath:string)) =
        PlayMusic.PlayWithOffsetFromPath offset filePath