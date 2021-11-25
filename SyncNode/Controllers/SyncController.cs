using Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SyncNode.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyncNode.Controllers
{
     [Route("[controller]")]
     [ApiController]
     public class SyncController : ControllerBase
     {
          private readonly SyncWorkJobService _workJobService;

          public SyncController(SyncWorkJobService workJobService)
          {
               _workJobService = workJobService;
          }

          [HttpPost]
          public IActionResult Sync(SyncEntity entity)//nod de sincronizare
          {

               _workJobService.AddItem(entity);
               // cand v-om primi o entitatea aceasta v-a fi pusa in que in background, 
               //iar un alt fir de executie v-a scoate toate aceste elemente si le va transmite mai departe la nodurile necesare
               return Ok();
          }
     }
}
