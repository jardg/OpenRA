#region Copyright & License Information
/*
 * Copyright 2007-2013 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation. For more information,
 * see COPYING.
 */
#endregion

using System;
using System.Drawing;
using System.Linq;
using OpenRA.Widgets;
using OpenRA.Network;

namespace OpenRA.Mods.Cnc.Widgets.Logic
{
	public class SpawnSelectorTooltipLogic
	{
		[ObjectCreator.UseCtor]
		public SpawnSelectorTooltipLogic(Widget widget, TooltipContainerWidget tooltipContainer, MapPreviewWidget preview)
		{
			widget.IsVisible = () => preview.TooltipSpawnIndex != -1;
			var label = widget.Get<LabelWidget>("LABEL");
			var flag = widget.Get<ImageWidget>("FLAG");
			var team = widget.Get<LabelWidget>("TEAM");

			var ownerFont = Game.Renderer.Fonts[label.Font];
			var teamFont = Game.Renderer.Fonts[team.Font];
			var cachedWidth = 0;
			var labelText = "";
			string playerCountry = null;
			var playerTeam = -1;

			tooltipContainer.BeforeRender = () =>
			{
				var client = preview.SpawnClients().Values.FirstOrDefault(c => c.SpawnPoint == preview.TooltipSpawnIndex);

				var teamWidth = 0;
				if (client == null)
				{
					labelText = "Available spawn";
					playerCountry = null;
					playerTeam = 0;
					widget.Bounds.Height = 25;
				}
				else
				{
					labelText = client.Name;
					playerCountry = client.Country;
					playerTeam = client.Team;
					widget.Bounds.Height = playerTeam > 0 ? 40 : 25;
					teamWidth = teamFont.Measure(team.GetText()).X;
				}

				label.Bounds.X = playerCountry != null ? flag.Bounds.Right + 5 : 5;

				var textWidth = ownerFont.Measure(labelText).X;
				if (textWidth != cachedWidth)
				{
					label.Bounds.Width = textWidth;
					widget.Bounds.Width = 2*label.Bounds.X + textWidth;
				}

				widget.Bounds.Width = Math.Max(teamWidth + 10, label.Bounds.Right + 5);
				team.Bounds.Width = widget.Bounds.Width;
			};

			label.GetText = () => labelText;
			flag.IsVisible = () => playerCountry != null;
			flag.GetImageCollection = () => "flags";
			flag.GetImageName = () => playerCountry;
			team.GetText = () => "Team {0}".F(playerTeam);
			team.IsVisible = () => playerTeam > 0;
		}
	}
}

