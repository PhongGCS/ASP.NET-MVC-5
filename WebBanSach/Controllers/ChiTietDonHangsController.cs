using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebBanSach.Models;
using PagedList;
using PagedList.Mvc;

namespace WebBanSach.Controllers
{
    public class ChiTietDonHangsController : Controller
    {
        private QuanLyBanSachEntities db = new QuanLyBanSachEntities();

        // GET: ChiTietDonHangs
        public ActionResult Index(int? idDonHang, int? page)
        {
            if (Session["TenKH"] == null || !Session["TenKH"].ToString().Equals("admin"))
                return RedirectToAction("DangNhap", "NguoiDung");
                var chiTietDonHangs = db.ChiTietDonHangs.Include(c => c.DonHang).Include(c => c.Sach);
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            return View(db.ChiTietDonHangs.ToList().Where(c => c.MaDonHang == idDonHang).OrderBy(n => n.MaDonHang).ToPagedList(pageNumber, pageSize));
        }

        // GET: ChiTietDonHangs/Details/5
        public ActionResult Details(int? idDH, int? idMS)
        {
            if (idDH == null || idMS == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChiTietDonHang chiTietDonHang = db.ChiTietDonHangs.Where(n=>n.MaDonHang==idDH && n.MaSach==idMS).SingleOrDefault();
            if (chiTietDonHang == null)
            {
                return HttpNotFound();
            }
            return View(chiTietDonHang);
        }

        // GET: ChiTietDonHangs/Create
        public ActionResult Create()
        {
            ViewBag.MaDonHang = new SelectList(db.DonHangs, "MaDonHang", "DaThanhToan");
            ViewBag.MaSach = new SelectList(db.Saches, "MaSach", "TenSach");
            return View();
        }

        // POST: ChiTietDonHangs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaDonHang,MaSach,SoLuong,DonGia")] ChiTietDonHang chiTietDonHang)
        {
            if (ModelState.IsValid)
            {
                db.ChiTietDonHangs.Add(chiTietDonHang);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaDonHang = new SelectList(db.DonHangs, "MaDonHang", "DaThanhToan", chiTietDonHang.MaDonHang);
            ViewBag.MaSach = new SelectList(db.Saches, "MaSach", "TenSach", chiTietDonHang.MaSach);
            return View(chiTietDonHang);
        }

        // GET: ChiTietDonHangs/Edit/5
        public ActionResult Edit(int? idDH, int? idMS)
        {
            if (idDH == null || idMS == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChiTietDonHang chiTietDonHang = db.ChiTietDonHangs.Where(n => n.MaDonHang == idDH && n.MaSach == idMS).SingleOrDefault(); 
            if (chiTietDonHang == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaDonHang = new SelectList(db.DonHangs, "MaDonHang", "DaThanhToan", chiTietDonHang.MaDonHang);
            ViewBag.MaSach = new SelectList(db.Saches, "MaSach", "TenSach", chiTietDonHang.MaSach);
            return View(chiTietDonHang);
        }

        // POST: ChiTietDonHangs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaDonHang,MaSach,SoLuong,DonGia")] ChiTietDonHang chiTietDonHang)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chiTietDonHang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaDonHang = new SelectList(db.DonHangs, "MaDonHang", "DaThanhToan", chiTietDonHang.MaDonHang);
            ViewBag.MaSach = new SelectList(db.Saches, "MaSach", "TenSach", chiTietDonHang.MaSach);
            return View(chiTietDonHang);
        }

       

        // GET: ChiTietDonHangs/Delete/5
        public ActionResult Delete(int? idDH, int? idMS)
        {
            if (idDH == null || idMS == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChiTietDonHang chiTietDonHang = db.ChiTietDonHangs.Where(n => n.MaDonHang == idDH && n.MaSach == idMS).SingleOrDefault();
            if (chiTietDonHang == null)
            {
                return HttpNotFound();
            }
            return View(chiTietDonHang);
        }

        // POST: ChiTietDonHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ChiTietDonHang chiTietDonHang = db.ChiTietDonHangs.Find(id);
            db.ChiTietDonHangs.Remove(chiTietDonHang);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult CapNhatDonGiaChiTiet(int iMaSP, FormCollection f)
        {
            //Kiểm tra masp
            Sach sach = db.Saches.SingleOrDefault(n => n.MaSach == iMaSP);
            //Nếu get sai masp thì sẽ trả về trang lỗi 404
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Lấy giỏ hàng ra từ session
            List<GioHang> lstGioHang = LayGioHang();
            //Kiểm tra sp có tồn tại trong session["GioHang"]
            GioHang sanpham = lstGioHang.SingleOrDefault(n => n.iMaSach == iMaSP);
            //Nếu mà tồn tại thì chúng ta cho sửa số lượng
            if (sanpham != null)
            {
                sanpham.iSoLuong = int.Parse(f["txtSoLuong"].ToString());

            }
            return RedirectToAction("GioHang");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


    }
}
