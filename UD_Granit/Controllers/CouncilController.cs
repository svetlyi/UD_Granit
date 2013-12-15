﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UD_Granit.Models;
using UD_Granit.Helpers;
using System.Web.Mvc.Html;
using System.Data.SqlClient;

namespace UD_Granit.Controllers
{
    // Управляет логикой по работе с информацией о диссертационном совете
    public class CouncilController : Controller
    {
        private DataContext db = new DataContext();

        // Показывает информацию о диссертационном совете
        // GET: /Council/

        public ActionResult Index()
        {
            UD_Granit.ViewModels.Council.Index viewModel = new ViewModels.Council.Index();
            viewModel.HaveInformation = (db.Council.Count() > 0);
            if (viewModel.HaveInformation)
                viewModel.Council = db.Council.First();
            if (Session.GetUser() is Administrator)
                viewModel.CanControl = true;
            return View(viewModel);
        }

        // Показывает форму редактирования информации о диссертационном совете
        // GET: /Council/Edit

        public ActionResult Edit()
        {
            User currentUser = Session.GetUser();

            if (currentUser == null)
                return HttpNotFound();
            if ((currentUser is Administrator) ||
                (Session.GetUserPosition() == MemberPosition.Chairman))
            {
                UD_Granit.ViewModels.Council.Edit viewModel = new ViewModels.Council.Edit();
                if (db.Council.Count() > 0)
                    viewModel.Council = db.Council.First();
                return View(viewModel);
            }
            return HttpNotFound();
        }

        // Редактирует информацию о диссертационном совете
        // POST: /Council/Edit

        [HttpPost]
        public ActionResult Edit(UD_Granit.ViewModels.Council.Edit viewModel)
        {
            try
            {
                if (db.Council.Count() == 0)
                {
                    db.Council.Add(viewModel.Council);
                }
                else
                {
                    viewModel.Council.Id = 1;
                    db.Entry(viewModel.Council).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
                HttpContext.Application["Name"] = viewModel.Council.Number;

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // Показывает членов диссертационного совета
        // GET: /Council/Members

        public ActionResult Members(string filter)
        {
            UD_Granit.ViewModels.Council.Members viewModel = new ViewModels.Council.Members();

            if (filter == null)
            {
                var q = from m in db.Members orderby m.Position descending select m;
                List<UD_Granit.ViewModels.Council.MemberView> memberList = new List<ViewModels.Council.MemberView>();
                foreach (Member m in q)
                {
                    memberList.Add(new UD_Granit.ViewModels.Council.MemberView() { Name = m.GetFullName(), Degree = m.Degree, Position = m.Position.ToDescription(), Speciality = m.Speciality.GetFullName(), Id = m.Id });
                }

                viewModel.CouncilMembers = memberList;
            }
            else
            {
                switch (filter)
                {
                    case "speciality":
                        var specialities = db.Specialities;
                        if (specialities != null)
                        {
                            viewModel.FilteredMembers = new Dictionary<string, IEnumerable<ViewModels.Council.MemberView>>();
                            foreach (var speciality in specialities)
                            {
                                var members = db.Members.Where(m => (m.Speciality.Number == speciality.Number));
                                if (members.Count() > 0)
                                {
                                    List<UD_Granit.ViewModels.Council.MemberView> memberList = new List<ViewModels.Council.MemberView>();
                                    foreach (Member m in members)
                                    {
                                        memberList.Add(new UD_Granit.ViewModels.Council.MemberView() { Name = m.GetFullName(), Degree = m.Degree, Position = m.Position.ToDescription(), Speciality = m.Speciality.GetFullName(), Id = m.Id });
                                    }
                                    viewModel.FilteredMembers.Add(speciality.GetFullName(), memberList);
                                }
                            }
                        }
                        break;
                    case "scienceBranch":
                        var branches = db.Database.SqlQuery<string>("GetScienceBranches");
                        if (branches != null)
                        {
                            viewModel.FilteredMembers = new Dictionary<string, IEnumerable<ViewModels.Council.MemberView>>();
                            foreach (var scienceBranch in branches)
                            {
                                var members = db.Database.SqlQuery<Member>("GetMembersByScienceBranch @Branch", new SqlParameter("Branch", scienceBranch));

                                List<UD_Granit.ViewModels.Council.MemberView> memberList = new List<ViewModels.Council.MemberView>();
                                foreach (Member m in members)
                                {
                                    m.Speciality = db.Members.Find(m.Id).Speciality;
                                    memberList.Add(new UD_Granit.ViewModels.Council.MemberView() { Name = m.GetFullName(), Degree = m.Degree, Position = m.Position.ToDescription(), Speciality = m.Speciality.GetFullName(), Id = m.Id });
                                }
                                viewModel.FilteredMembers.Add(scienceBranch, memberList);

                                if (memberList.Count == 0)
                                    viewModel.FilteredMembers.Remove(scienceBranch);
                            }
                        }
                        break;
                }
            }

            viewModel.CanControl = RightsManager.Account.Edit(Session.GetUser());

            return View(viewModel);
        }
    }
}
