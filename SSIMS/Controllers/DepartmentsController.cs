﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SSIMS.Database;
using SSIMS.DAL;
using SSIMS.Models;
using System.Diagnostics;
using SSIMS.Filters;
using SSIMS.Service;
using SSIMS.ViewModels;


namespace SSIMS.Controllers
{
    [AuthenticationFilter]
    [AuthorizationFilter]
    public class DepartmentsController : Controller
    {
        
        UnitOfWork unitOfWork = new UnitOfWork();
        ILoginService loginService = new LoginService();
        DepartmentService departmentService = new DepartmentService();
        IStaffService staffService = new StaffService();
        // GET: Departments
        public ActionResult Index()
        {
            Debug.WriteLine("Hey drake , user type = " + Session["usertype"]);
            Debug.WriteLine("Hey drake , user group = " + Session["usergroup"]);
            var departments = unitOfWork.DepartmentRepository.Get(includeProperties: "CollectionPoint");
            ViewBag.RepList = unitOfWork.StaffRepository.GetDeptRepList();
            Debug.WriteLine("number of heads: " + unitOfWork.StaffRepository.GetDeptHeadList().Count());
            var Deps = unitOfWork.DepartmentRepository.Get(filter: x => x.ID != "STOR", includeProperties: "DeptHeadAuthorization.Staff");
            ViewBag.Departments = Deps;
            List<string> authNames = new List<string>();
            foreach (Department d in Deps)
            {
                if (d.DeptHeadAuthorization == null)
                    authNames.Add("none");
                else
                    authNames.Add(d.DeptHeadAuthorization.Staff.Name);
            }
            ViewBag.HeadList = unitOfWork.StaffRepository.Get(filter: x => x.StaffRole == "DeptHead");
            ViewBag.RepList = unitOfWork.StaffRepository.Get(filter: x => x.StaffRole == "DeptRep");
            ViewBag.Auth = authNames;
            ViewBag.DeptCount = unitOfWork.DepartmentRepository.Get().Count();
            return View(departments.ToList());
        }

        // GET: Departments/Details/5
        public ActionResult Details(string id)
        {
            Debug.WriteLine("welcome to department details, routevalue id = " + id);
            if (String.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            
            Department department = unitOfWork.DepartmentRepository.Get(filter: x => x.ID == id, includeProperties: "CollectionPoint").First();
                
            CollectionPoint selected = department.CollectionPoint;
            ViewBag.SelectedPoint = selected.Location;
            ViewBag.OtherPoints = unitOfWork.CollectionPointRepository.Get(filter: x => x.Location != selected.Location);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (department == null)
            {
                return HttpNotFound();
            }
            ViewBag.Department = department;
            Session["CurrentDepartmentID"] = department.ID;
            return View();
        }

        [HttpPost]
        public ActionResult UpdateCollectionPoint(string id)
        {
            string deptId = Session["CurrentDepartmentID"].ToString();
            Department department = unitOfWork.DepartmentRepository.GetByID(deptId);
            CollectionPoint collectionPoint = unitOfWork.CollectionPointRepository.GetByID(int.Parse(id));
            if (collectionPoint != null) {
                department.CollectionPoint = collectionPoint;
                unitOfWork.DepartmentRepository.Update(department);
                unitOfWork.Save();
                Debug.WriteLine("Collection point for " + department.DeptName + " update to " + collectionPoint.Location);
            }
            return RedirectToAction("Details", new { id = deptId });
        }

        // GET: Departments/Create 
        public ActionResult Create()
        {
            ViewBag.CollectionPointID = new SelectList(unitOfWork.CollectionPointRepository.Get(), "ID", "Location");
            ViewBag.DeptHeadID = new SelectList(unitOfWork.StaffRepository.Get(), "ID", "UserAccountID");
            ViewBag.DeptRepID = new SelectList(unitOfWork.StaffRepository.Get(), "ID", "UserAccountID");
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,DeptRepID,DeptHeadID,CollectionPointID,DeptHeadAutorizationID,DeptName,PhoneNumber,FaxNumber")] Department department)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.DepartmentRepository.Update(department);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            ViewBag.CollectionPointID = new SelectList(unitOfWork.CollectionPointRepository.Get(), "ID", "Location", department.CollectionPoint.ID);
            ViewBag.DeptHeadID = new SelectList(unitOfWork.StaffRepository.Get(), "ID", "UserAccountID", department.DeptHead.ID);
            ViewBag.DeptRepID = new SelectList(unitOfWork.StaffRepository.Get(), "ID", "UserAccountID", department.DeptRep.ID);
            return View(department);
        }

        // GET: Departments/Edit/ARCH
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = unitOfWork.DepartmentRepository.GetByID(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            ViewBag.CollectionPointID = new SelectList(unitOfWork.CollectionPointRepository.Get(), "ID", "Location", department.CollectionPoint.ID);
            ViewBag.DeptHeadID = new SelectList(unitOfWork.StaffRepository.Get(), "ID", "UserAccountID", department.DeptHead.ID);
            ViewBag.DeptRepID = new SelectList(unitOfWork.StaffRepository.Get(), "ID", "UserAccountID", department.DeptRep.ID);
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,DeptRepID,DeptHeadID,CollectionPointID,DeptHeadAutorizationID,DeptName,PhoneNumber,FaxNumber")] Department department)
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            if (ModelState.IsValid)
            {
                unitOfWork.DepartmentRepository.Update(department);
                unitOfWork.Save();
                return RedirectToAction("Index");

            }
            ViewBag.CollectionPointID = new SelectList(unitOfWork.CollectionPointRepository.Get(), "ID", "Location", department.CollectionPoint.ID);
            ViewBag.DeptHeadID = new SelectList(unitOfWork.StaffRepository.Get(), "ID", "UserAccountID", department.DeptHead.ID);
            ViewBag.DeptRepID = new SelectList(unitOfWork.StaffRepository.Get(), "ID", "UserAccountID", department.DeptRep.ID);

            //ViewBag.CollectionPointID = new SelectList(unitOfWork.CollectionPointRepository.Get(), "ID", "Location", department.CollectionPoint.ID);
            //ViewBag.DeptHeadID = new SelectList(unitOfWork.StaffRepository.Get(), "UserAccountID", department.DeptHead.ID);
            //ViewBag.DeptRepID = new SelectList(unitOfWork.StaffRepository.Get(), "UserAccountID", department.DeptRep.ID);
            return View(department);
        }

        // GET: Departments/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = unitOfWork.DepartmentRepository.GetByID(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Department department = unitOfWork.DepartmentRepository.GetByID(id);
            unitOfWork.DepartmentRepository.Delete(department);
            unitOfWork.Save();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult SelectCollectionPoint (string id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = unitOfWork.DepartmentRepository.GetByID(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            ViewBag.CollectionPointID = new SelectList(unitOfWork.CollectionPointRepository.Get(), "ID", "Location", department.CollectionPoint.ID);
            return View(department);
        }


        //Go to view for dep head only to select staff for delegation
        public ActionResult DelegateAuthority()
        {
            string dept = loginService.StaffFromSession.DepartmentID;
            //show history and form together
            //get a list of auths (history)
            List<DeptHeadAuthVM> vmlist = departmentService.GetDeptHeadAuthorizationVMs(dept);
            if(vmlist == null)
            {
                Debug.WriteLine("EMPTY AUTHLIST!!");
            }
            //get a list of staff (new delegation)
            List<Staff> stafflist = staffService.GetStaffByDeptID(dept);
            ViewBag.StaffList = stafflist;
            DeptHeadAuthorization auth = new DeptHeadAuthorization();
            //get current active delegation (if exists)
            if(departmentService.IsActiveAuthExist(dept, out auth))
            {
                ViewBag.CurrentDelegation = new DeptHeadAuthVM(auth);
            } else
            {
                ViewBag.CurrentDelegation = null;
            }
            return View(vmlist);
        }


        //behind the scene to process the submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DelegateAuthority([Bind(Include = "ID,StartDate,EndDate,DepartmentID")] DeptHeadAuthorization deptHeadAuthorization)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.DeptHeadAuthorizationRepository.Insert(deptHeadAuthorization);
                unitOfWork.Save();
                return RedirectToAction("DelegateAuthority");
            }
            return RedirectToAction("Details", new { id = loginService.StaffFromSession.DepartmentID});
        }


        //End Delegation
    }
}

