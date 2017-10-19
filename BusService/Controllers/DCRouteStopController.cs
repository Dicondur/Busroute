using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusService.Models;
using Microsoft.AspNetCore.Http;

namespace BusService.Controllers
{
    public class DCRouteStopController : Controller
    {
        private readonly BusServiceContext _context;

        public DCRouteStopController(BusServiceContext context)
        {
            _context = context;
        }

        // GET: RouteStop
        public async Task<IActionResult> Index(string busRouteCode)
        {
            var _busRouteCode = busRouteCode;

            if (busRouteCode != null)
            {
                HttpContext.Session.SetString("__busRouteCode", busRouteCode);

                TempData["message"] = "Stored to session";

            }
            else if ((busRouteCode == null) && (HttpContext.Session.GetString("__busRouteCode")!=null))
            {
                _busRouteCode = HttpContext.Session.GetString("__busRouteCode");

                TempData["message"] = "Got from session";
            }
            else if (HttpContext.Session.GetString("__busRouteCode") == null)
            {
                TempData["message"] = "Please select a Bus route to show the RouteStops";
                return RedirectToAction("Index", "DCBusRoute");
            }



            var busServiceContext = _context.RouteStop
                                .Include(r => r.BusRouteCodeNavigation)
                                .Include(r => r.BusStopNumberNavigation)
                                //.Include(x=> x.BusStopNumber)
                                .Where(x => x.BusRouteCode == _busRouteCode)
                                .OrderBy(x=> x.OffsetMinutes);
            //if (busServiceContext.Count() != 0) { 
            ViewBag.Route = _busRouteCode;
            ViewBag.RouteName = _context.BusRoute
                                .Include(r => r.RouteStop)
                                .Where(x => x.BusRouteCode == _busRouteCode)
                                .SingleOrDefault().RouteName;


           
            //}
            return View(await busServiceContext.ToListAsync());
        }

        // GET: RouteStop/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeStop = await _context.RouteStop
                .Include(r => r.BusRouteCodeNavigation)
                .Include(r => r.BusStopNumberNavigation)
                .SingleOrDefaultAsync(m => m.RouteStopId == id);
            if (routeStop == null)
            {
                return NotFound();
            }

            return View(routeStop);
        }

        // GET: RouteStop/Create
        public IActionResult Create()
        {
            ViewData["BusRouteCode"] = new SelectList(_context.BusRoute, "BusRouteCode", "BusRouteCode");
            ViewData["BusStopNumber"] = new SelectList(_context.BusStop.OrderBy(x=>x.Location), "BusStopNumber", "Location");

            var _busRouteCode = "";
            if (HttpContext.Session.GetString("__busRouteCode") != null)
            {
                _busRouteCode = HttpContext.Session.GetString("__busRouteCode");
                TempData["message"] = "Got __BusRouteCode from session" + _busRouteCode;
                //return RedirectToAction("Index", "DCBusRoute");
            }


            ViewBag.Route = _context.BusRoute
                                    .Where(x=>x.BusRouteCode == _busRouteCode)
                                    .FirstOrDefault()
                                    .BusRouteCode;
            ViewBag.RouteName = _context.BusRoute
                                    .Where(x => x.BusRouteCode == _busRouteCode)
                                    .FirstOrDefault()
                                    .RouteName;
            return View();
        }

        // POST: RouteStop/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RouteStopId,BusRouteCode,BusStopNumber,OffsetMinutes")] RouteStop routeStop)
        {

            routeStop.BusRouteCode = HttpContext.Session.GetString("__busRouteCode");//get from session

            //condition ii
            var condition2 = _context.RouteStop
                                    .Where(x=>x.BusRouteCode == routeStop.BusRouteCode)
                                    .Where(x => x.OffsetMinutes == 0)
                                    .Count();

            //condition i
            if (routeStop.OffsetMinutes < 0 || routeStop.OffsetMinutes == null)
            {

                TempData["message"] = "Condition 1 dint pass";
                ModelState.AddModelError("OffsetMinutes", "OffsetMinutes has to be >0 ");
            }
            else
            {


                TempData["message"] = "Condition 1 passed";
            }



            //condition ii
            if (condition2 < 1) //shld return true for route 100 
            {
                //Has no start in db
                
                
                if(routeStop.OffsetMinutes !=0) //Has no start in input
                {
                TempData["message"] = "Please enter a start position";
                ModelState.AddModelError("OffsetMinutes", "There must be a 0 OffsetMinutes");
                }


            }
            else if (condition2 == 1)
            {
                //Has one start already in db
                if (routeStop.OffsetMinutes == 0)
                {
                    ModelState.AddModelError("OffsetMinutes", "There is a 0 OffsetMinutes already");

                }
            }
            
            {
                TempData["message"] = "Condition 2 passed";
            }


            //condition iii

            var condition3 = (_context.RouteStop
                                    .Where(x => x.BusStopNumber == routeStop.BusStopNumber)
                                    .Where(x => x.BusRouteCode == routeStop.BusRouteCode)
                                    .Count() );


            if (condition3 >= 1)
            {

                TempData["message"] = "The bus stop for this route already exists";
                ModelState.AddModelError("OffsetMinutes", "BusStopNumber for this route already exists");
            }

            else
            {
                TempData["message"] = "Condition 3 passed";
            }

            //Step iv
            if (ModelState.IsValid)
            {
                _context.Add(routeStop);
                await _context.SaveChangesAsync();
                TempData["message"] = "Added to list";
                return RedirectToAction(nameof(Index));
            }
            


            ViewData["BusRouteCode"] = new SelectList(_context.BusRoute, "BusRouteCode", "BusRouteCode", routeStop.BusRouteCode);
            ViewData["BusStopNumber"] = new SelectList(_context.BusStop.OrderBy(x => x.Location), "BusStopNumber", "Location", routeStop.BusStopNumber);



            ViewBag.Route = _context.BusRoute
                                    .Where(x => x.BusRouteCode == routeStop.BusRouteCode)
                                    .FirstOrDefault()
                                    .BusRouteCode;
            ViewBag.RouteName = _context.BusRoute
                                    .Where(x => x.BusRouteCode == routeStop.BusRouteCode)
                                    .FirstOrDefault()
                                    .RouteName;


            return View(routeStop);//Step5
        }
                                        
        // GET: RouteStop/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeStop = await _context.RouteStop.SingleOrDefaultAsync(m => m.RouteStopId == id);
            if (routeStop == null)
            {
                return NotFound();
            }
            ViewData["BusRouteCode"] = new SelectList(_context.BusRoute, "BusRouteCode", "BusRouteCode", routeStop.BusRouteCode);
            ViewData["BusStopNumber"] = new SelectList(_context.BusStop, "BusStopNumber", "BusStopNumber", routeStop.BusStopNumber);
            return View(routeStop);
        }

        // POST: RouteStop/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RouteStopId,BusRouteCode,BusStopNumber,OffsetMinutes")] RouteStop routeStop)
        {
            if (id != routeStop.RouteStopId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(routeStop);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RouteStopExists(routeStop.RouteStopId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BusRouteCode"] = new SelectList(_context.BusRoute, "BusRouteCode", "BusRouteCode", routeStop.BusRouteCode);
            ViewData["BusStopNumber"] = new SelectList(_context.BusStop, "BusStopNumber", "BusStopNumber", routeStop.BusStopNumber);
            return View(routeStop);
        }

        // GET: RouteStop/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeStop = await _context.RouteStop
                .Include(r => r.BusRouteCodeNavigation)
                .Include(r => r.BusStopNumberNavigation)
                .SingleOrDefaultAsync(m => m.RouteStopId == id);
            if (routeStop == null)
            {
                return NotFound();
            }

            return View(routeStop);
        }

        // POST: RouteStop/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var routeStop = await _context.RouteStop.SingleOrDefaultAsync(m => m.RouteStopId == id);
            _context.RouteStop.Remove(routeStop);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RouteStopExists(int id)
        {
            return _context.RouteStop.Any(e => e.RouteStopId == id);
        }
    }
}
