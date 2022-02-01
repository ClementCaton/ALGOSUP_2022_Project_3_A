namespace Synthesizer

open System
open System.IO

module Synth =
    
    let GetNoteFreq Octav Note =
        CalcNoteFreq(Octav, Note).Output

    let GetNoteFreqOffset Octav Note AFourFreq =
        CalcNoteFreq(Octav, Note, AFourFreq).Output

    let Sound Freq Duration WaveType =
        let Data = SoundData(Frequency0 = Freq, Duration0 = Duration, Bpm0 = 114) // TEMP: Remove bpm
        //! The "1." was supposed to be "(data.overDrive)"
        Utility.Overdrive 1. (Data.Create(WaveType))

    let SoundWithEnveloppe Freq Duration WaveType Sustain Attack Hold Decay Release = // time, time, time, amp, time
        let Data = SoundData(Frequency0 = Dreq, Duration0 = Duration, Bpm0 = 114) // TEMP: Remove bpm
        //! The "1." was supposed to be "(data.overDrive)"
        Utility.Overdrive 1. (Data.CreateWithEnvelope WaveType Sustain Attack Hold Decay Release)

    let WriteToWav Name Music =
        Directory.CreateDirectory("./Output/") |> ignore
        use Stream = File.Create("./Output/" + name)
        WriteWav().Write (Stream) (Music)

    let WriteToWavWithPath Path FileName Music =
        Directory.CreateDirectory(Path) |> ignore
        use Stream = File.Create(Path + FileName)
        WriteWav().Write (Stream) (Music)

    let ReadFromWav Name =
        ReadWav().Read (File.Open("./Output/"+Name, FileMode.Open))

    let ReadFromWavWithPath Path =
        ReadWav().Read (File.Open(Path, FileMode.Open))

    let Note Duration MNote Octave =
        let Freq = GetNoteFreq MNote Octave
        Sound Freq Duration Sin

    
    let Silence Duration =
        Sound 0 Duration Silence
        
    let ComposeCutCorner (Corner:int) Sounds =
        Sounds |> List.map(fun x -> Utility.CutCorners Corner X) |> List.concat
            
    let Compose = ComposeWithCutCorner 100
    
    let ComposeNoCutCorner Sounds = List.concat
    
    let Add Sounds = Utility.Add Sounds

    let Preview Title Sound =
        Previewarr.chart Title Sound
        Sound

    let PreviewMap Title Map =
        Map
        |> Map.toList
        |> List.unzip
        ||> Previewarr.chartXY Title
        Map

    let ForAllChannels Func Channels =
        Channels |> List.map func

    let Fourier Wave =
        FrequencyAnalysis.Fourier(Wave)

    let Cutstart (SampleRate:float) Time (Data:List<float>) =
        Utility.CutStart SampleRate Time Data

    let CutEnd (SampleRate:float) Time (Data:List<float>) =
        Utility.CutEnd SampleRate Time Data

    let CutMiddle (SampleRate:float) TimeStart TimeEnd (Data:List<float>) =
        Utility.CutStart SampleRate TimeStart Data
        |> List.append (Utility.CutEnd SampleRate TimeEnd Data)

    let CutEdge (SampleRate:float) TimeStart TimeEnd (Data:List<float>) =
        Utility.CutEnd SampleRate TimeStart Data
        |> List.append (Utility.CutStart SampleRate TimeEnd Data)
        
    let CutCorners Limit (Data:List<float>) =
        Utility.CutCorners Limit Data

    
    