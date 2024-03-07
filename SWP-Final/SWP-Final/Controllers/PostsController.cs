using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP_Final.Entities;
using SWP_Final.Models;

namespace SWP_Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly RealEasteSWPContext _context;

        public PostsController(RealEasteSWPContext context)
        {
            _context = context;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            if (_context.Posts == null)
            {
                return NotFound();
            }

            var postlist = await _context.Posts.ToListAsync();

            // Check if the post list is empty
            if (postlist.Count == 0)
            {
                return NotFound("No posts found.");
            }

            bool changesMade = false;
            foreach (var post in postlist)
            {
                if (post.Images == null || post.Images.Length == 0)
                {
                    post.Images = "Images/common/noimage.png"; // Update with your default image path
                    changesMade = true;
                }
            }

            // Save changes if any post was updated
            if (changesMade)
            {
                await _context.SaveChangesAsync();

            }
            return await _context.Posts.ToListAsync();
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(string id)
        {
            if (_context.Posts == null)
            {
                return NotFound();
            }
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(string id, Post post)
        {
            if (id != post.PostId)
            {
                return BadRequest();
            }

            _context.Entry(post).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Posts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Post>> PostPost(Post post)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'RealEasteSWPContext.Posts'  is null.");
            }
            _context.Posts.Add(post);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PostExists(post.PostId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPost", new { id = post.PostId }, post);
        }

        // DELETE: api/Posts/5
        [HttpDelete("DeletePost/{id}")]
        public async Task<IActionResult> DeletePost(string id)
        {
            if (_context.Posts == null)
            {
                return NotFound();
            }
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //GET: api/Posts/GetImage/
        [HttpGet("GetImage/{id}")]
        public async Task<IActionResult> GetImage(string id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null || string.IsNullOrEmpty(post.Images))
            {
                return NotFound("The image does not exist or has been deleted.");
            }

            var path = GetFilePath(post.Images);
            if (!System.IO.File.Exists(path))
            {
                return NotFound("File does not exist.");
            }

            var imageStream = System.IO.File.OpenRead(path);


            var mimeType = "image/jpeg";
            if (Path.GetExtension(path).ToLower() == ".png")
            {
                mimeType = "image/png";
            }
            else if (Path.GetExtension(path).ToLower() == ".gif")
            {
                mimeType = "image/gif";
            }
            return File(imageStream, mimeType);
        }



        //POST: api/Posts/PostImage
        [HttpPost("PostInfoWithImage")]
        public async Task<IActionResult> PostInfoWithImageAsync([FromForm] PostModel postModel)
        {
            string fileNameImagePostModel = "Images/PostImage/" + postModel.FileImage.FileName;
            if (ModelState.IsValid)
            {
                var post = new Post
                {
                    PostId = postModel.PostId,


                };

                if (postModel.FileImage.Length > 0)
                {
                    var path = GetFilePath(fileNameImagePostModel);
                    using (var stream = System.IO.File.Create(path))
                    {
                        await postModel.FileImage.CopyToAsync(stream);
                    }
                    post.Images = fileNameImagePostModel;
                }

                _context.Posts.Add(post);
                await _context.SaveChangesAsync();

                return Ok(post);
            }
            return BadRequest("Invalid model state.");
        }

        //GET: api/Posts/UploadImageNoImage
        [HttpGet("UploadImageNoImage")]
        public async Task<IActionResult> UploadImageNoImage()
        {
            var postlist = await _context.Posts.ToListAsync();

            // Check if the post list is empty
            if (postlist.Count == 0)
            {
                return NotFound("No posts found.");
            }

            bool changesMade = false;
            foreach (var post in postlist)
            {
                if (post.Images == null || post.Images.Length == 0)
                {
                    post.Images = "Images/common/noimage.png"; // Update with your default image path
                    changesMade = true;
                }
            }

            // Save changes if any post was updated
            if (changesMade)
            {
                await _context.SaveChangesAsync();
                return Ok("Default images assigned to agencies without images.");
            }

            return Ok("No agencies needed updates.");
        }

        //POST: api/Posts/UploadImage
        [HttpPost("UploadInformationWithImage/{id}")]
        public async Task<IActionResult> UploadInformationWithImage([FromForm] PostModel postModel, string id)
        {
            int count = 0;
            var postlist = await _context.Posts.ToListAsync();
            string fileNameImagePostModel = $"Images/PostImage/{postModel.FileImage.FileName}";
            var post = await _context.Posts.FindAsync(id);
            string fileNameImagePost = post.Images;
            if (post == null)
            {
                return NotFound("Post not found");
            }
            foreach (var postimage in postlist)
            {
                if (postimage.Images == fileNameImagePost)
                {
                    count++;
                }
            }

            var path = GetFilePath(fileNameImagePost);
            if (System.IO.File.Exists(path))
            {
                if (fileNameImagePost != valiablenoimage() && count == 0)
                {
                    System.IO.File.Delete(path);
                }

            }
            var filepath = GetFilePath(fileNameImagePostModel);
            using (var stream = System.IO.File.Create(filepath))
            {
                await postModel.FileImage.CopyToAsync(stream);
            }
            post.Description = postModel.Description;
            post.PriorityMethod = postModel.PriorityMethod;
            post.Images = fileNameImagePostModel;


            await _context.SaveChangesAsync();
            return Ok(post);
        }

        //DELETE: api/Posts/DeleteImage
        [HttpDelete("DeleteImage/{id}")]
        public async Task<IActionResult> DeleteImage(string id)
        {
            int count = 0;
            var postlist = await _context.Posts.ToListAsync();
            var post = await _context.Posts.FindAsync(id);
            string fileNameImagePost = post.Images;
            if (post == null || string.IsNullOrEmpty(post.Images) || post.Images == valiablenoimage())
            {
                return NotFound("Post not found or image already removed.");
            }

            foreach (var postimage in postlist)
            {
                if (postimage.Images == fileNameImagePost)
                {
                    count++;
                }
            }

            if (count != 0)
            {
                post.Images = valiablenoimage();
                await _context.SaveChangesAsync();
                return Ok("Image successfully deleted.");
            }

            var path = GetFilePath(post.Images);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                // Optionally, update the post object to reflect that the image has been removed
                post.Images = null; // Assuming 'Images' is the property holding the image path. Adjust if necessary.
                _context.Posts.Update(post);
                await _context.SaveChangesAsync();
                return Ok("Image successfully deleted.");
            }
            else
            {
                return NotFound("File does not exist.");
            }
        }


        [NonAction]

        private string valiablenoimage() => "Images/common/noimage.png";

        private string GetFilePath(string filename) => Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filename);

        private bool PostExists(string id)
        {
            return (_context.Posts?.Any(e => e.PostId == id)).GetValueOrDefault();
        }
    }
}
