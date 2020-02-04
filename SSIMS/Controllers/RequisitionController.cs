﻿using SSIMS.DAL;
using SSIMS.Service;
using SSIMS.Models;
using SSIMS.ViewModels;
using SSIMS.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Status = SSIMS.Models.Status;
using PagedList;


namespace SSIMS.Controllers
{
    [AuthenticationFilter]
    [AuthorizationFilter]
    public class RequisitionController : Controller
    {
        //DatabaseContext db = new DatabaseContext();

        readonly IRequisitionService RequisitionService = new RequisitionService();
        readonly IStaffService StaffService = new StaffService();
        //for json implementation of dynamic html
        //public JsonResult GetDocumentItems()
        //{
            
        //    var qry = new List<RequisitionItemVM>();//need to according to staff
        //    //qry = ritems;
        //    return Json(qry, JsonRequestBehavior.AllowGet);
        //}


        [HttpPost]
        public ActionResult Add(string SelectedCategory, string SelectedDescription, string Quantity)
        {
            UnitOfWork uni = new UnitOfWork();

            if(Session["RequestList"] == null)
                Session["RequestList"] = new List<RequisitionItemVM>();
            List<RequisitionItemVM> requisitionItems = (List<RequisitionItemVM>)Session["RequestList"];

            if (ModelState.IsValid)
            {
                
                RequisitionItemVM requisitionItem = new RequisitionItemVM { SelectedCategory = SelectedCategory, SelectedDescription = SelectedDescription, Quantity = int.Parse(Quantity) };
                requisitionItem.UnitOfMeasure = uni.ItemRepository.GetByID(SelectedDescription).UnitOfMeasure;
                requisitionItem.displayDescription = uni.ItemRepository.GetByID(SelectedDescription).Description;

                requisitionItems.Add(requisitionItem);
                Debug.WriteLine("Added item to list, going back to create");
            }
            Session["RequestList"] = requisitionItems;
            return RedirectToAction("Create","Requisition");
        }

        //public class RequisitionCreateBinder : IModelBinder
        //{
        //    public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        //    {  //throw new NotImplementedException();
        //        HttpContextBase objContext = controllerContext.HttpContext;
        //        String category = objContext.Request.Form["SelectedCategory"];
        //        String description= objContext.Request.Form["SelectedDescription"];//return item ID
        //        int quantity = Convert.ToInt32(objContext.Request.Form["Quantity"]);
        //        RequisitionItemVM objrivm = new RequisitionItemVM
        //        {
        //            ItemID=description,
        //            SelectedCategory = category,
        //            Quantity=quantity
        //        };
               

        //        return objrivm;
        //    }

        //}








        // GET: Requisition
        public ActionResult Index(Staff staff, int? page)
        {
            staff.ID = 10006;
            var unitofwork = new UnitOfWork();

            staff = unitofwork.StaffRepository.GetStaffbyID(staff.ID);

            Debug.WriteLine(staff.ID);
            Debug.WriteLine(staff.Name);
            //Debug.WriteLine(staff.Department.DeptName);

            var reqList = unitofwork.RequisitionOrderRepository.Get(filter: x => x.CreatedByStaffID == staff.ID).ToList();

            //var reqList2 = RequisitionService.GetRequisitionOrdersbyStatus(staff, searchString);

            //ViewBag.SearchString = searchString;
            
            int pageSize =10;
            int pageNumber = (page ?? 1);

            return View(reqList.ToPagedList(pageNumber, pageSize));


        }

        //private ActionResult View(object p)
        //{
        //    throw new NotImplementedException();
        //}

        // GET: Requisition/ViewHistory
        public ActionResult ViewHistory(int? page)
        {

            var unitofwork = new UnitOfWork();

            var reqList = unitofwork.RequisitionOrderRepository.Get(filter: x => x.Status == Models.Status.Approved|| x.Status == Models.Status.Rejected);

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(reqList.ToPagedList(pageNumber, pageSize));

        }

        //GET:Requisition/Manage
        public ActionResult Manage(int? page)
        {
            var unitofwork = new UnitOfWork();
            var reqList = unitofwork.RequisitionOrderRepository.Get(filter: x => x.Status == Models.Status.Pending);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(reqList.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult GetDescription(string category)
        {
            var unitofwork = new UnitOfWork();
            var irepo = unitofwork.ItemRepository;

            IEnumerable<SelectListItem> descriptionlist = (IEnumerable<SelectListItem>)irepo.GetDescriptions(category);
            return Json(descriptionlist, JsonRequestBehavior.AllowGet);

        }

        // GET: Requisition/Create
        public ActionResult Create2()
        {
            //for create dynamic dropdownlist
            var unitofwork = new UnitOfWork();
            var irepo = unitofwork.ItemRepository;
            RequisitionItemVM vm = new RequisitionItemVM();
            vm.Categories = irepo.GetCategories();
            vm.Descriptions = irepo.GetDescriptions();

            return View(vm);
        }
        // GET: Requisition/Create
        public ActionResult Create()
        {
            Debug.WriteLine("Hello im in create now");
            //Session["RequestList"] = null;
            RequisitionItemVM vm = new RequisitionItemVM();
            List<RequisitionItemVM> list = (List < RequisitionItemVM >) Session["RequestList"];
            //Debug.WriteLine("Session list has " + list.Count + "items");
            //Debug.WriteLine(list.Count);
            ViewBag.RequisitionItems = list;
            if (list == null)
            //for create dynamic dropdownlist
            {
                List<RequisitionItemVM> vmList = new List<RequisitionItemVM>();
                vmList.Add(vm);
                Session["RequestList"] = vmList;
                ViewBag.RequisitionItems = vmList;
            }
            
            return View(vm);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitRequisition()
        {
            var unitofwork = new UnitOfWork();
            List<RequisitionItemVM> vmList =(List<RequisitionItemVM>) Session["RequestList"];
            //RequisitionItemVM rcvm = new RequisitionItemVM;
            Debug.WriteLine(vmList.Count);
            foreach(RequisitionItemVM vm in vmList)
            {
                Debug.WriteLine(vm.Quantity);
            }

            List<DocumentItem> doitems = new List<DocumentItem>();
            if (ModelState.IsValid)
            {
                foreach (RequisitionItemVM rcvm in vmList)
                {
                    if (rcvm.Quantity != 0) { 
                    Item item = unitofwork.ItemRepository.GetItembyDescrption(rcvm.SelectedDescription);
                    DocumentItem doitem = unitofwork.DocumentItemRepository.InsertDocumentItembyItemandQty(item, rcvm.Quantity);
                    
                    doitems.Add(doitem);
                    unitofwork.Save();
                    }
                }

                RequisitionOrder rq = new RequisitionOrder(unitofwork.StaffRepository.GetByID(10006));
                //rq.DocumentItems.Add(unitofwork.DocumentItemRepository.GetByID(doitem.ID));

                rq.DocumentItems = doitems;

                //RequisitionService.CreateNewRequistionOrder(vmList, unitofwork.StaffRepository.GetByID(10006));
                unitofwork.RequisitionOrderRepository.Insert(rq);

                unitofwork.Save();
                Debug.WriteLine("rq.ID:"+rq.ID);

                //List<RequisitionItemVM> vmList11 = (List<RequisitionItemVM>)Session["RequestList"];
                //Debug.WriteLine("before session cleaar:list.count:"+ vmList11.Count);
                Session["RequestList"] = null;
                //List<RequisitionItemVM> vmList22 = (List<RequisitionItemVM>)Session["RequestList"];
                //Debug.WriteLine("after session cleaar:list.count:"+ vmList22.Count);
                return RedirectToAction("Index");
            }

            return View();
        }

        // POST: Requisition/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create2([Bind(Include = "SelectedCategory,SelectedDescription,Quantity")] RequisitionItemVM rcvm)
        {
            var unitofwork = new UnitOfWork();
            Debug.WriteLine(rcvm.SelectedCategory + "\n" + rcvm.SelectedDescription + " x " + rcvm.Quantity.ToString());
            Debug.WriteLine(rcvm.CreatedDate);
            Debug.WriteLine(rcvm.Status);
            // Debug.WriteLine(rcvm.RequisitionOrderID);
            if (ModelState.IsValid)
            {

                //get item by itemID
                Item item = unitofwork.ItemRepository.GetItembyDescrption(rcvm.SelectedDescription);
                Debug.WriteLine(item.ID);

                //save this item into the doucumentitem(item, qty)
                DocumentItem doitem = unitofwork.DocumentItemRepository.InsertDocumentItembyItemandQty(item, rcvm.Quantity);
                unitofwork.Save();
                Debug.WriteLine(doitem.ID);

                RequisitionOrder rq = new RequisitionOrder(unitofwork.StaffRepository.GetByID(10006));
                rq.DocumentItems.Add(unitofwork.DocumentItemRepository.GetByID(doitem.ID));

                unitofwork.RequisitionOrderRepository.Insert(rq);

                unitofwork.Save();

                return RedirectToAction("Index");
            }

            return View(rcvm);
        }


        // GET: Requisition/Details/4
        public ActionResult Details(int id)
        {
            //return documentitem by input requisitionOrder ID
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var unitofwork = new UnitOfWork();

            var requisitionOrder = unitofwork.RequisitionOrderRepository.Get(filter: x => x.ID == id, includeProperties: "DocumentItems.Item").FirstOrDefault();


            if (requisitionOrder == null)
            {
                return HttpNotFound();
            }
            return View(requisitionOrder);

        }

        // GET: Requisition/Edit/4
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Debug.WriteLine("Edit get");

            // //delete requisitionOrder by ID
            // var unitofwork = new UnitOfWork();
            // RequisitionOrder ro = unitofwork.RequisitionOrderRepository.GetByID(id);
            // var doitems = ro.DocumentItems.ToList();
            // unitofwork.DocumentItemRepository.Delete(doitems);
            // Debug.WriteLine(ro.ID);
            //  //unitofwork.RequisitionOrderRepository.DeleteRObyID(id);

            // //unitofwork.RequisitionOrderRepository.Delete(ro);



            // unitofwork.Save();
            // if (ro.ID == null)
            // {
            //     Debug.WriteLine(" delete");
            // }
            // else
            // {
            //     Debug.WriteLine("not delete :"+ro.ID);
            // }

            var unitofwork = new UnitOfWork();
            var irepo = unitofwork.ItemRepository;
            RequisitionItemVM vm = new RequisitionItemVM();
            vm.Categories = irepo.GetCategories();
            vm.Descriptions = irepo.GetDescriptions();

            vm.ROID = id;



            return View(vm);
        }

        // POST: Requisition/Edit/4
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SelectedCategory,SelectedDescription,Quantity,ROID")] RequisitionItemVM rcvm)
        {
            var unitofwork = new UnitOfWork();
            Debug.WriteLine(rcvm.SelectedCategory + "\n" + rcvm.SelectedDescription + " x " + rcvm.Quantity.ToString());

            Debug.WriteLine(rcvm.ROID);
            // Debug.WriteLine(rcvm.RequisitionOrderID);
            if (ModelState.IsValid)
            {

                //get item by itemID
                Item item = unitofwork.ItemRepository.GetItembyDescrption(rcvm.SelectedDescription);
                Debug.WriteLine(item.ID);

                //save this item into the doucumentitem(item, qty)
                DocumentItem doitem = unitofwork.DocumentItemRepository.InsertDocumentItembyItemandQty(item, rcvm.Quantity);

                //doitem.Document.ID = rcvm.ROID;
                unitofwork.Save();
                Debug.WriteLine(doitem.ID);

                RequisitionOrder rq = new RequisitionOrder(unitofwork.StaffRepository.GetByID(10006));
                rq.DocumentItems.Add(unitofwork.DocumentItemRepository.GetByID(doitem.ID));
                rq.ID = rcvm.ROID;
                Debug.WriteLine(rq.ID);


                unitofwork.RequisitionOrderRepository.Insert(rq);
                Debug.WriteLine(rq.ID);

                unitofwork.Save();
                Debug.WriteLine(rq.ID);

                return RedirectToAction("Index");
            }

            return View(rcvm);
        }

        //GET: Requisition/Cancel/4
        public ActionResult Cancel(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var unitofwork = new UnitOfWork();

            var requisitionOrder = unitofwork.RequisitionOrderRepository.Get(filter: x => x.ID == id, includeProperties: "DocumentItems.Item").FirstOrDefault();
            requisitionOrder.Status = Models.Status.Cancelled;
            unitofwork.Save();

            if (requisitionOrder == null)
            {
                return HttpNotFound();
            }
            return View(requisitionOrder);

        }

        //GET: Requisition/Approve/4
        public ActionResult Approve(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var unitofwork = new UnitOfWork();

            var requisitionOrder = unitofwork.RequisitionOrderRepository.Get(filter: x => x.ID == id, includeProperties: "DocumentItems.Item").FirstOrDefault();
            requisitionOrder.Status = Models.Status.Approved;
            requisitionOrder.ResponseDate = DateTime.Now;
            unitofwork.Save();

            if (requisitionOrder == null)
            {
                return HttpNotFound();
            }
            return View(requisitionOrder);
        }


        //GET: Requisition/Reject/4
        public ActionResult Reject(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var unitofwork = new UnitOfWork();

            var requisitionOrder = unitofwork.RequisitionOrderRepository.Get(filter: x => x.ID == id, includeProperties: "DocumentItems.Item").FirstOrDefault();
            requisitionOrder.Status = Models.Status.Rejected;
            requisitionOrder.ResponseDate = DateTime.Now;
            unitofwork.Save();

            if (requisitionOrder == null)
            {
                return HttpNotFound();
            }
            return View(requisitionOrder);
        }

    }

    

}
