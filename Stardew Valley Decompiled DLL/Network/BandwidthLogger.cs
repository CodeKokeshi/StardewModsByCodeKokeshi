// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.BandwidthLogger
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Network;

public class BandwidthLogger
{
  private long bitsDownSinceLastUpdate;
  private long bitsUpSinceLastUpdate;
  private DateTime lastUpdateTime = DateTime.UtcNow;
  private double lastBitsDownPerSecond;
  private double lastBitsUpPerSecond;
  private double avgBitsUpPerSecond;
  private long bitsUpPerSecondCount;
  private double avgBitsDownPerSecond;
  private long bitsDownPerSecondCount;
  private long totalBitsDown;
  private long totalBitsUp;
  private double totalMs;
  private int queueCapacity = 100;
  private Queue<double> bitsUp = new Queue<double>();
  private Queue<double> bitsDown = new Queue<double>();

  public void Update()
  {
    double totalMilliseconds = (DateTime.UtcNow - this.lastUpdateTime).TotalMilliseconds;
    if (totalMilliseconds <= 1000.0)
      return;
    this.lastBitsDownPerSecond = (double) this.bitsDownSinceLastUpdate / totalMilliseconds * 1000.0;
    this.lastBitsUpPerSecond = (double) this.bitsUpSinceLastUpdate / totalMilliseconds * 1000.0;
    this.avgBitsDownPerSecond = (this.avgBitsDownPerSecond * (double) this.bitsDownPerSecondCount + this.lastBitsDownPerSecond) / (double) ++this.bitsDownPerSecondCount;
    this.avgBitsUpPerSecond = (this.avgBitsUpPerSecond * (double) this.bitsUpPerSecondCount + this.lastBitsUpPerSecond) / (double) ++this.bitsUpPerSecondCount;
    this.lastUpdateTime = DateTime.UtcNow;
    this.bitsDownSinceLastUpdate = 0L;
    this.bitsUpSinceLastUpdate = 0L;
    this.totalMs += totalMilliseconds;
    if (this.bitsUp.Count >= this.queueCapacity)
      this.bitsUp.Dequeue();
    if (this.bitsDown.Count >= this.queueCapacity)
      this.bitsDown.Dequeue();
    this.bitsUp.Enqueue(this.lastBitsUpPerSecond);
    this.bitsDown.Enqueue(this.lastBitsDownPerSecond);
  }

  public double AvgBitsDownPerSecond => this.avgBitsDownPerSecond;

  public double AvgBitsUpPerSecond => this.avgBitsUpPerSecond;

  public double BitsDownPerSecond => this.lastBitsDownPerSecond;

  public double BitsUpPerSecond => this.lastBitsUpPerSecond;

  public double TotalBitsDown => (double) this.totalBitsDown;

  public double TotalBitsUp => (double) this.totalBitsUp;

  public double TotalMs => this.totalMs;

  public Queue<double> LoggedAvgBitsUp => this.bitsUp;

  public Queue<double> LoggedAvgBitsDown => this.bitsDown;

  public void RecordBytesDown(long bytes)
  {
    this.bitsDownSinceLastUpdate += bytes * 8L;
    this.totalBitsDown += bytes * 8L;
  }

  public void RecordBytesUp(long bytes)
  {
    this.bitsUpSinceLastUpdate += bytes * 8L;
    this.totalBitsUp += bytes * 8L;
  }
}
