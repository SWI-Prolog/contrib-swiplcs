///*********************************************************
//* 
//*  Author:        Foutelet Joel
//*
//*  This library is free software; you can redistribute it and/or
//*  modify it under the terms of the GNU Lesser General Public
//*  License as published by the Free Software Foundation; either
//*  version 2.1 of the License, or (at your option) any later version.
//*
//*  This library is distributed in the hope that it will be useful,
//*  but WITHOUT ANY WARRANTY; without even the implied warranty of
//*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//*  Lesser General Public License for more details.
//*
//*  You should have received a copy of the GNU Lesser General Public
//*  License along with this library; if not, write to the Free Software
//*  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//*
//*********************************************************/


//#region demo_doc_fs
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
//#endregion demo_doc_fs
