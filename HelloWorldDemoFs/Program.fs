// Learn more about F# at http://fsharp.net
open System
open SbsSW.SwiPlCs;

let ple = PlEngine.IsInitialized in
       if ple then printfn "Echec initialisation" else
         begin
             PlEngine.Initialize([|"-q"|])
             PlQuery.PlCall("assert(father(martin, inka))") |> ignore
             PlQuery.PlCall("assert(father(uwe, gloria))") |> ignore
             PlQuery.PlCall("assert(father(uwe, melanie))") |> ignore
             PlQuery.PlCall("assert(father(uwe, ayala))") |> ignore

             let q = new PlQuery "father(P, C), atomic_list_concat([P,'is_father_of ',C], L)" in
             begin
                 Seq.iter (fun (x : PlQueryVariables)  -> printfn "%A" (x.Item("L").Name)) q.SolutionVariables

                 printfn "all children from uwe:"
                 let r : PlQueryVariables = q.Variables in r.Item("P").Unify("uwe") |> ignore
                 Seq.iter (fun (x : PlQueryVariables)  -> printfn "%A" (x.Item("C").Name)) q.SolutionVariables
             end

             PlEngine.PlCleanup()
             printfn "%A" "finished!"
         end

