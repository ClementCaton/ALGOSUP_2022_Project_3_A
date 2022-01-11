namespace Synthesizer

// module preview
//     open XPlot.Plotly
//     let chart1 sampleRate freq amplitude overdrive =
//         let abc = 
//             float >> (fun x -> (x / float sampleRate)) >> fourWaves.sinWave freq amplitude >> overD.makeOverdrive overdrive
//         Array.init sampleRate abc
//         |> Chart.Line |> Chart.WithOptions(Options(title = "sinusoïdal")) |> Chart.Show

//     let chart2 sampleRate freq amplitude overdrive =
//         let abc = 
//             float >> (fun x -> (x / float sampleRate)) >> fourWaves.sawWave freq amplitude >> overD.makeOverdrive overdrive
//         Array.init sampleRate abc
//         |> Chart.Line |> Chart.WithOptions(Options(title = "sinusoïdal")) |> Chart.Show

//     let chart3 sampleRate freq amplitude overdrive =
//         let abc = 
//             float >> (fun x -> (x / float sampleRate)) >> fourWaves.triangleWave freq amplitude >> overD.makeOverdrive overdrive
//         Array.init sampleRate abc
//         |> Chart.Line |> Chart.WithOptions(Options(title = "sinusoïdal")) |> Chart.Show

//     let chart4 sampleRate freq amplitude overdrive =
//         let abc = 
//             float >> (fun x -> (x / float sampleRate)) >> fourWaves.squareWave freq amplitude >> overD.makeOverdrive overdrive
//         Array.init sampleRate abc
//         |> Chart.Line |> Chart.WithOptions(Options(title = "sinusoïdal")) |> Chart.Show
