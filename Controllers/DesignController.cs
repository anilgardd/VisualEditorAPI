using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisualEditorAPI.Data;
using VisualEditorAPI.Models;
using System.Text.Json;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace VisualEditorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DesignController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DesignController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("CreateDesign")]
        public async Task<IActionResult> CreateDesign([FromBody] CreateDesignDto request)
        {
            if (request.UserId <= 0 || request.Data?.CanvasObjects?.Data == null)
            {
                return BadRequest("Invalid request data.");
            }

            var design = new Design
            {
                UserId = request.UserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Components = request.Data.CanvasObjects.Data.Select(kvp => new Component
                {
                    Type = kvp.Value.Type,
                    Properties = JsonSerializer.Serialize(kvp.Value)
                }).ToList()
            };

            _context.Designs.Add(design);
            await _context.SaveChangesAsync();

            return Ok(design);
        }


        [HttpGet]
        [Route("GenerateReactComponent/{designId}")]
        public async Task<IActionResult> GenerateReactComponent(int designId)
        {
            var design = await _context.Designs
                .Include(d => d.Components)
                .FirstOrDefaultAsync(d => d.Id == designId);

            if (design == null)
            {
                return NotFound("Design not found.");
            }

            var sb = new StringBuilder();
            var cssSb = new StringBuilder();

            sb.AppendLine("import React from 'react';");
            sb.AppendLine();
            sb.AppendLine("const GeneratedComponent = () => {");
            sb.AppendLine("  return (");
            sb.AppendLine("    <div>");

            foreach (var component in design.Components)
            {
                var componentData = JsonSerializer.Deserialize<CanvasObjectDto>(component.Properties);
                var styles = GenerateStyles(componentData);
                var componentHtml = GenerateComponentHtml(componentData);

                sb.AppendLine($"      {componentHtml}");
                cssSb.AppendLine(styles);
            }

            sb.AppendLine("    </div>");
            sb.AppendLine("  );");
            sb.AppendLine("};");
            sb.AppendLine();
            sb.AppendLine("export default GeneratedComponent;");

            var response = new
            {
                ReactComponent = sb.ToString(),
                Css = cssSb.ToString()
            };

            return Ok(response);
        }
        [HttpGet]
        [Route("GetDesignsByUser/{userId}")]
        public async Task<IActionResult> GetDesignsByUser(int userId)
        {
            var designs = await _context.Designs
                .Where(d => d.UserId == userId)
                .Include(d => d.Components)
                .ToListAsync();

            if (designs == null || !designs.Any())
            {
                return NotFound("No designs found for the given user.");
            }

            return Ok(designs);
        }

        private string GenerateStyles(CanvasObjectDto component)
        {
            var sb = new StringBuilder();
            sb.AppendLine($".{component.ObjectId} {{");
            sb.AppendLine($"  position: absolute;");
            sb.AppendLine($"  left: {component.Left}px;");
            sb.AppendLine($"  top: {component.Top}px;");
            sb.AppendLine($"  width: {component.Width}px;");
            sb.AppendLine($"  height: {component.Height}px;");
            sb.AppendLine($"  background-color: {component.Fill};");
            if (component.Stroke != null)
            {
                sb.AppendLine($"  border: {component.StrokeWidth}px solid {component.Stroke};");
            }
            sb.AppendLine($"  transform: rotate({component.Angle}deg);");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateComponentHtml(CanvasObjectDto component)
        {
            var type = component.Type.ToLower();
            var html = string.Empty;

            switch (type)
            {
                case "rect":
                    html = $"<div className=\"{component.ObjectId}\"></div>";
                    break;
                case "circle":
                    html = $"<div className=\"{component.ObjectId}\" style=\"border-radius: 50%;\"></div>";
                    break;
                case "triangle":
                    html = $"<div className=\"{component.ObjectId}\" style=\"width: 0; height: 0; border-left: {component.Width / 2}px solid transparent; border-right: {component.Width / 2}px solid transparent; border-bottom: {component.Height}px solid {component.Fill};\"></div>";
                    break;
                case "line":
                    html = $"<div className=\"{component.ObjectId}\" style=\"border-top: {component.StrokeWidth}px solid {component.Stroke}; width: {component.Width}px;\"></div>";
                    break;
                case "i-text":
                    html = $"<div className=\"{component.ObjectId}\" style=\"font-family: {component.FontFamily}; font-size: {component.FontSize}px;\">{component.Text}</div>";
                    break;
                default:
                    html = $"<div className=\"{component.ObjectId}\"></div>";
                    break;
            }

            return html;
        }
    }
}
public class CreateDesignDto
{
    public int UserId { get; set; }
    public LiveblocksDataDto Data { get; set; }
}

public class LiveblocksDataDto
{
    public CanvasObjectsDto CanvasObjects { get; set; } = new CanvasObjectsDto();
}

public class CanvasObjectsDto
{
    public Dictionary<string, CanvasObjectDto> Data { get; set; } = new Dictionary<string, CanvasObjectDto>();
}

public class CanvasObjectDto
{
    public string Type { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string OriginX { get; set; } = string.Empty;
    public string OriginY { get; set; } = string.Empty;
    public float Left { get; set; }
    public float Top { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public string Fill { get; set; } = string.Empty;
    public string? Stroke { get; set; }
    public float StrokeWidth { get; set; }
    public string? StrokeDashArray { get; set; }
    public string StrokeLineCap { get; set; } = string.Empty;
    public float StrokeDashOffset { get; set; }
    public string StrokeLineJoin { get; set; } = string.Empty;
    public bool StrokeUniform { get; set; }
    public float StrokeMiterLimit { get; set; }
    public float ScaleX { get; set; }
    public float ScaleY { get; set; }
    public float Angle { get; set; }
    public bool FlipX { get; set; }
    public bool FlipY { get; set; }
    public float Opacity { get; set; }
    public string? Shadow { get; set; }
    public bool Visible { get; set; }
    public string BackgroundColor { get; set; } = string.Empty;
    public string FillRule { get; set; } = string.Empty;
    public string PaintFirst { get; set; } = string.Empty;
    public string GlobalCompositeOperation { get; set; } = string.Empty;
    public float SkewX { get; set; }
    public float SkewY { get; set; }
    public float Rx { get; set; }
    public float Ry { get; set; }
    public string ObjectId { get; set; } = string.Empty;
    public float Radius { get; set; }
    public float StartAngle { get; set; }
    public float EndAngle { get; set; }
    public float X1 { get; set; }
    public float X2 { get; set; }
    public float Y1 { get; set; }
    public float Y2 { get; set; }
    public string FontFamily { get; set; } = string.Empty;
    public string FontWeight { get; set; } = string.Empty;
    public float FontSize { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool Underline { get; set; }
    public bool Overline { get; set; }
    public bool Linethrough { get; set; }
    public string TextAlign { get; set; } = string.Empty;
    public string FontStyle { get; set; } = string.Empty;
    public float LineHeight { get; set; }
    public string TextBackgroundColor { get; set; } = string.Empty;
    public float CharSpacing { get; set; }
    public List<string> Styles { get; set; } = new List<string>();
    public string Direction { get; set; } = string.Empty;
    public string? Path { get; set; }
    public float PathStartOffset { get; set; }
    public string PathSide { get; set; } = string.Empty;
    public string PathAlign { get; set; } = string.Empty;
}
