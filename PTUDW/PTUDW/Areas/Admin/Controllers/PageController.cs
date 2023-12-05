using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using MyClass.DAO;
using MyClass.Model;
using PTUDW.Library;

namespace PTUDW.Areas.Admin.Controllers
{
    public class PageController : Controller
    {
        PostsDAO postsDAO = new PostsDAO();
        LinksDAO linksDAO = new LinksDAO();
        TopicsDAO topicsDAO = new TopicsDAO();
        //INDEX
        // GET: Admin/Category
        public ActionResult Index()
        {
            return View(postsDAO.getList("Index","Page"));
        }

        //DETAIL/////////////////////////////////////////////
        // GET: Admin/Category/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tồn tại trang đơn");
                return RedirectToAction("Index");
            }
            Posts posts = postsDAO.getRow(id);
            if (posts == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tồn tại trang đơn");
                return RedirectToAction("Index");
            }
            return View(posts);
        }

        /////////////////////////////////////////////////////////////////////////////////////
        // GET: Admin/Post/Create: Them moi mot mau tin
        public ActionResult Create()
        {
            //ViewBag.TopList = new SelectList(topicsDAO.getList("Index"), "Id", "Name");
            return View();
        }

        // POST: Admin/Post/Create: Them moi trang don
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Posts posts)
        {
            if (ModelState.IsValid)
            {
                //Xu ly cho muc Slug
                posts.Slug = XString.Str_Slug(posts.Title);
                //chuyen doi dua vao truong Name de loai bo dau, khoang cach = dau -

                //xu ly cho phan upload hình ảnh
                var img = Request.Files["img"];//lay thong tin file
                if (img.ContentLength != 0)
                {
                    string[] FileExtentions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    //kiem tra tap tin co hay khong
                    if (FileExtentions.Contains(img.FileName.Substring(img.FileName.LastIndexOf("."))))
                    //lay phan mo rong cua tap tin
                    {
                        string slug = posts.Slug;
                        string id = posts.Id.ToString();
                        //Chinh sua sau khi phat hien dieu chua dung cua Edit: them Id
                        //ten file = Slug + Id + phan mo rong cua tap tin
                        string imgName = slug + id + img.FileName.Substring(img.FileName.LastIndexOf("."));
                        posts.Img = imgName;

                        string PathDir = "~/Public/img/page/";
                        string PathFile = Path.Combine(Server.MapPath(PathDir), imgName);
                        //upload hinh
                        img.SaveAs(PathFile);
                    }
                }//ket thuc phan upload hinh anh

                //xu ly cho muc PostType
                posts.PostType = "page";

                //Xu ly cho muc CreateAt
                posts.CreateAt = DateTime.Now;

                //Xu ly cho muc CreateBy
                posts.CreateBy = Convert.ToInt32(Session["UserId"]);

                //xu ly cho muc Topics
                if (postsDAO.Insert(posts) == 1)//khi them du lieu thanh cong
                {
                    Links links = new Links();
                    links.Slug = posts.Slug;
                    links.TableId = posts.Id;
                    links.Type = "page";
                    linksDAO.Insert(links);
                }
                //Thong bao thanh cong
                TempData["message"] = new XMessage("success", "Thêm trang đơn thành công");
                return RedirectToAction("Index");
            }
            ViewBag.TopList = new SelectList(topicsDAO.getList("Index"), "Id", "Name");
            return View(posts);
        }

        //EDIT
        // GET: Admin/Category/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tìm thấy trang đơn");
                return RedirectToAction("Index");
            }
            Posts posts = postsDAO.getRow(id);
            if (posts == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tìm thấy trang đơn");
                return RedirectToAction("Index");
            }
            //ViewBag.TopList = new SelectList(topicsDAO.getList("Index"), "Id", "Name");
            return View(posts);
        }

        // POST: Admin/Post/Edit/5: Cap nhat mau tin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Posts posts)
        {
            if (ModelState.IsValid)
            {
                //Xu ly cho muc Slug
                posts.Slug = XString.Str_Slug(posts.Title);
                //chuyen doi dua vao truong Name de loai bo dau, khoang cach = dau -

                //xu ly cho phan upload hình ảnh
                var img = Request.Files["img"];//lay thong tin file
                string PathDir = "~/Public/img/page/";
                if (img.ContentLength != 0)
                {
                    //Xu ly cho muc xoa hinh anh
                    if (posts.Img != null)
                    {
                        string DelPath = Path.Combine(Server.MapPath(PathDir), posts.Img);
                        System.IO.File.Delete(DelPath);
                    }

                    string[] FileExtentions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    //kiem tra tap tin co hay khong
                    if (FileExtentions.Contains(img.FileName.Substring(img.FileName.LastIndexOf("."))))
                    //lay phan mo rong cua tap tin
                    {
                        string slug = posts.Slug;
                        //Chinh sua sau khi phat hien dieu chua dung cua Edit: them Id
                        //ten file = Slug + phan mo rong cua tap tin
                        string imgName = slug + img.FileName.Substring(img.FileName.LastIndexOf("."));
                        posts.Img = imgName;
                        //upload hinh
                        string PathFile = Path.Combine(Server.MapPath(PathDir), imgName);
                        img.SaveAs(PathFile);
                    }
                }//ket thuc phan upload hinh anh

                //Xu ly cho muc UpdateAt
                posts.UpdateAt = DateTime.Now;

                //Xu ly cho muc UpdateBy
                posts.UpdateBy = Convert.ToInt32(Session["UserId"]);

                //xu ly cho muc Links
                if (postsDAO.Update(posts) == 1)//khi sua du lieu thanh cong
                {
                    Links links = new Links();
                    links.Slug = posts.Slug;
                    links.TableId = posts.Id;
                    links.Type = "page";
                    linksDAO.Insert(links);
                }
                //Thong bao thanh cong
                TempData["message"] = new XMessage("success", "Sửa trang đơn thành công");
                return RedirectToAction("Index");
            }
            //ViewBag.TopList = new SelectList(topicsDAO.getList("Index"), "Id", "Name");
            return View(posts);
        }

        //DELETE
        // GET: Admin/Category/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Xóa mẫu tin thất bại");
                return RedirectToAction("Index");
            }
            Posts posts = new Posts();
            if (posts == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Xóa mẫu tin thất bại");
                return RedirectToAction("Index");
            }
            return View(posts);
        }

        // POST: Admin/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Posts posts = postsDAO.getRow(id);
            //tim thay mau tin thi xoa, cap nhat cho Links
            if (postsDAO.Delete(posts) == 1)
            {
                Links links = linksDAO.getRow(posts.Id, "page");
                //Xoa luon cho Links
                linksDAO.Delete(links);

                //duong dan den anh can xoa
                string PathDir = "~/Public/img/page/";
                //cap nhat thi phai xoa file cu
                if (posts.Img != null)
                {
                    string DelPath = Path.Combine(Server.MapPath(PathDir), posts.Img);
                    System.IO.File.Delete(DelPath);
                }
            }
            //Thong bao thanh cong
            TempData["message"] = TempData["message"] = new XMessage("success", "Xóa mẫu tin thành công");
            return RedirectToAction("Trash");
        }

        //STATUS
        // GET: Admin/Category/Status/5
        public ActionResult Status(int? id)
        {
            if (id == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
            }
            //Truy van Id
            Posts posts = postsDAO.getRow(id);
            if (posts == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
            }
            else
            {
                //Chuyen doi trang thai cua Status 1 <- -> 2
                posts.Status = (posts.Status == 1) ? 2 : 1;

                //cap nhat gia tri cho UpdateAt/By
                posts.UpdateBy = Convert.ToInt32(Session["UserId"].ToString());
                posts.UpdateAt = DateTime.Now;

                //Cap nhat lai DB
                postsDAO.Update(posts);

                //Thong bao cap nhat trang thai thanh cong
                TempData["message"] = TempData["message"] = new XMessage("success", "Cập nhật trạng thái thành công");

                return RedirectToAction("Index");
            }
        }
        //Deltrash
        // GET: Admin/Category/Deltrash/5
        public ActionResult DelTrash(int? id)
        {
            if (id == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tìm thấy mẫu tin");
                return RedirectToAction("Index");
            }
            //Truy van Id
            Posts posts = postsDAO.getRow(id);
            if (posts == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tìm thấy mẫu tin");
                return RedirectToAction("Index");
            }
            else
            {
                //Chuyen doi trang thai cua Status 1,2 -> 0 : không hiển thị ở Index
                posts.Status = 0;

                //cap nhat gia tri cho UpdateAt/By
                posts.UpdateBy = Convert.ToInt32(Session["UserId"].ToString());
                posts.UpdateAt = DateTime.Now;
                    
                //Cap nhat lai DB
                postsDAO.Update(posts);

                //Thong bao cap nhat trang thai thanh cong
                TempData["message"] = TempData["message"] = new XMessage("success", "Xóa trang đơn thành công");

                return RedirectToAction("Index");
            }
        }

        //Trash
        // GET: Admin/Trash
        public ActionResult Trash()
        {
            return View(postsDAO.getList("Trash","page"));
        }

        //RECOVER       
        // GET: Admin/Category/Recover/5
        public ActionResult Recover(int? id)
        {
            if (id == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Phục hồi mẫu tin thất bại");
                return RedirectToAction("Index");
            }
            //Truy van Id
            Posts posts = postsDAO.getRow(id);
            if (posts == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Phục hồi mẫu tin thất bại");
                return RedirectToAction("Index");
            }
            else
            {
                //Chuyen doi trang thai cua Status 0-->2 : Khong xuat ban
                posts.Status = 2;

                //cap nhat gia tri cho UpdateAt/By
                posts.UpdateBy = Convert.ToInt32(Session["UserId"].ToString());
                posts.UpdateAt = DateTime.Now;

                //Cap nhat lai DB
                postsDAO.Update(posts);

                //Thong bao mau tin trang thai thanh cong
                TempData["message"] = TempData["message"] = new XMessage("success", "Phục hồi mẫu tin thành công");

                return RedirectToAction("Index");
            }
        }
    }
}
